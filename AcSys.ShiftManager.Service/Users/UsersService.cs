using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using AcSys.Core.Data.Querying;
using AcSys.Core.Email;
using AcSys.Core.Extensions;
using AcSys.Core.ObjectMapping;
using AcSys.ShiftManager.Data.EF.Identity;
using AcSys.ShiftManager.Data.Messages;
using AcSys.ShiftManager.Data.Notifications;
using AcSys.ShiftManager.Data.UnitOfWork;
using AcSys.ShiftManager.Data.Users;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Model.Helpers;
using AcSys.ShiftManager.Service.Base;
using AcSys.ShiftManager.Service.Common;
using AcSys.ShiftManager.Service.Results;
using Autofac.Extras.NLog;
using AcSys.Core.Data.Model.Base;

namespace AcSys.ShiftManager.Service.Users
{
    public class UsersService : ApplicationServiceBase, IUsersService
    {
        protected IRoleRepository RoleRepo { get; set; }
        protected IUserRepository Repo { get; set; }
        protected INotificationRepository NotificationRepo { get; set; }
        protected IMessageRepository MessageRepo { get; set; }

        protected IEmployeeGroupRepository EmpGroupRepo { get; set; }

        public UsersService(
            IUnitOfWork unitOfWork,
            ApplicationRoleManager roleManager,
            ApplicationUserManager userManager,
            IUserRepository repo,
            IEmployeeGroupRepository empGroupRepo,
            IRoleRepository roleRepo,
            INotificationRepository notificationRepo, 
            IMessageRepository messageRepo,
            ILogger logger,
            IEmailService emailService
            ) : base(unitOfWork, roleManager, userManager, logger, emailService)
        {
            //_db = db;
            Repo = repo;
            RoleRepo = roleRepo;
            EmpGroupRepo = empGroupRepo;
            NotificationRepo = notificationRepo;
            MessageRepo = messageRepo;

            // wrong
            //Logger = LogManager.GetCurrentClassLogger();

            //Logger = logger;

            //Logger.Trace("Sample trace message");
            //Logger.Debug("Sample debug message");
            //Logger.Info("Sample informational message");
            //Logger.Warn("Sample warning message");
            //Logger.Error("Sample error message");
            //Logger.Fatal("Sample fatal error message");

            //// alternatively you can call the Log() method and pass log level as the parameter.
            //Logger.Log(LogLevel.Info, "Sample informational message");

            //throw new ApplicationException("Me");
        }

        #region Roles Methods

        public async Task<List<RoleDto>> GetRoles()
        {
            //var roles = await RoleManager.Roles.ToListAsync();
            var roles = await RoleRepo.GetAllAsync();
            var roleDtos = ObjectMapper.Map<List<Role>, List<RoleDto>>(roles);
            return roleDtos;
        }

        public async Task<RoleDto> GetRole(Guid id)
        {
            Role role = await RoleManager.FindByIdAsync(id);
            if (role == null) NotFound();

            var dto = ObjectMapper.Map<Role, RoleDto>(role);
            return dto;
        }

        #endregion

        public UserDto Me()
        {
            //throw new ApplicationException("Elmah test error.");

            //var errors = GetElmahErrors();
            //var errorsCount = errors.Count();

            string repoName = Repo.GetType().FullName;

            UserDto dto = ObjectMapper.Map<User, UserDto>(LoggedInUser);
            Role role = LoggedInUser.GetRole();
            if (role != null)
            {
                //dto.Role = role.Name;
                dto.Role = ObjectMapper.Map<RoleDto>(role);
            }

            Logger.Info("Got Own User Info!");

            return dto;
        }

        public async Task<IListResult<User, UserDto>> GetUsers(FindUsersQuery query)
        {
            //string repoName = MessageRepo.GetType().FullName;

            //Logger.Trace("{0} viewed list of users.", LoggedInUser.Email);

            //var users = await UserManager.Users.Where(o => o.EntityStatus != Enums.EntityStatus.Deleted).ToListAsync();
            
            //var searchResults = await Repo.Find(query.ToSpec());
            var searchResults = await Repo.Find(query);

            IListResult<User, UserDto> results = new ListResult<User, UserDto>(query, searchResults);
            
            LoggedInUser.AddLog(Enums.SubjectType.User,
                query.SearchCriteria.IsNullOrWhiteSpace() ? Enums.ActivityType.Viewed : Enums.ActivityType.Searched,
                query.SearchCriteria.IsNullOrWhiteSpace() ? string.Empty : "Searched for: {0}".FormatWith(query.SearchCriteria));

            await UnitOfWork.SaveChangesAsync();

            return results;
        }

        public async Task<UserDto> GetUser(Guid id)
        {
            //User user = await _db.Users.FindAsync(id);
            //User user = await UserManager.Users.FirstOrDefaultAsync(o => o.Id == id);
            User user = await Repo.FindAsync(id);

            //TODO: Handle Not Found Exception in ExceptionFilter
            //if (user == null) return NotFound();
            if (user == null) NotFound();

            var userDto = ObjectMapper.Map<User, UserDto>(user);

            LoggedInUser.AddLog(Enums.ActivityType.Viewed, user);
            await UnitOfWork.SaveChangesAsync();

            return userDto;
        }

        async Task<bool> UserExists(Guid id)
        {
            return await Repo.CountAsync(e => e.Id == id) > 0;
        }

        public async Task<Guid> CreateUser(UserDto dto)
        {
            CheckIfPropertyHasValue("Email", dto.Email);
            CheckIfPropertyHasValue("UserName", dto.UserName);
            CheckIfEntityHasId("User role", dto.Role);

            await CheckIfUserNameIsTaken(dto);
            await CheckIfEmailIsTaken(dto);

            User user = ObjectMapper.Map<UserDto, User>(dto);
            //user.UserName = user.Email;

            Role role = await GetRole(dto);
            user.AddRole(role);

            await SetEmployeeGroup(dto, user);

            await CreatePassword(user, dto.Password);

            user.SecurityStamp = Guid.NewGuid().ToString();

            Repo.Add(user);

            await UnitOfWork.SaveChangesAsync();

            LoggedInUser.AddLog(Enums.ActivityType.Created, user);
            await UnitOfWork.SaveChangesAsync();

            return user.Id;
        }

        public async Task UpdateUser(Guid id, UserDto dto)
        {
            //if (!ModelState.IsValid) return BadRequest(ModelState);
            //if (id != userDto.Id) return BadRequest();

            LoggedInUserShouldHaveAnyRoleIn(AppConstants.RoleNames.SuperAdmin, AppConstants.RoleNames.Admin, AppConstants.RoleNames.HRManager, AppConstants.RoleNames.RecManager);

            User user = await Repo.FindByIdAsync(id);
            if (user == null) NotFound();

            await UpdateUserPassword(user, dto.Password);

            // username cannot be changed
            if (dto.UserName.IsNullOrWhiteSpace())
                dto.UserName = user.UserName;
            else if (dto.UserName != user.UserName)
                BadRequest("UserName cannot be changed once a user has been created.");

            if (dto.Email != user.Email)
                await CheckIfEmailIsTaken(dto);

            if (dto.Role == null || dto.Role.HasNoId)
                BadRequest("User role must be specified.");

            Role currentRole = user.GetRole();
            if (dto.Role.Id.Value != currentRole.Id)
            {
                Role newRole = await RoleManager.FindByIdAsync(dto.Role.Id.Value);
                user.RemoveRoles();
                if (user.DoesNotHaveRole(newRole))
                {
                    user.AddRole(newRole);
                }
            }

            await UpdateEmployeeGroup(user, dto);

            ObjectMapper.Map(dto, user);

            Repo.Update(user);

            LoggedInUser.AddLog(Enums.ActivityType.Updated, user);

            try
            {
                await UnitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await UserExists(id)))
                {
                    NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        async Task CreatePassword(User user, string password)
        {
            if (password.IsNotNullOrWhiteSpace())
            {
                if (LoggedInUserHasNoRoleIn(AppConstants.RoleNames.SuperAdmin, AppConstants.RoleNames.Admin, AppConstants.RoleNames.HRManager, AppConstants.RoleNames.RecManager))
                    Forbidden("Your are not authrorised to set the user's password.");

                await SetUserPassword(user, password);
            }
            else
            {
                await SetUserPassword(user, "P@ssw0rd");
                user.MustChangePassword = true;
            }
        }

        async Task UpdateUserPassword(User user, string password)
        {
            if (password.IsNullOrWhiteSpace()) return;

            //if (LoggedInUserHasNoRoleIn(AppConstants.RoleNames.SuperAdmin, AppConstants.RoleNames.Admin)
            //        && LoggedInUserIsNot(user))
            //    Unauthorized();
            if (LoggedInUserIsNot(user))
            {
                if (user.HasRole(AppConstants.RoleNames.SuperAdmin))
                {
                    //LoggedInUserShouldHaveRoles(AppConstants.RoleNames.SuperAdmin);
                    BadRequest("You are not authorized to set SuperAdmin user's password.");
                }
                else
                    LoggedInUserShouldHaveAnyRoleIn(AppConstants.RoleNames.SuperAdmin, AppConstants.RoleNames.Admin);
            }

            await SetUserPassword(user, password);
        }

        async Task SetUserPassword(User user, string password)
        {
            //IdentityResult result = await UserManager.CreateAsync(user, "P@ssw0rd");
            //if (!result.Succeeded)
            //    throw new ApplicationException(result.Errors.FirstOrDefault());

            var result = await UserManager.PasswordValidator.ValidateAsync(password);
            if (result.Succeeded)
            {
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
            }
            else
            {
                BadRequest(result.Errors.FirstOrDefault());
            }
        }

        async Task SetEmployeeGroup(UserDto dto, User user)
        {
            EmployeeGroup empGroup = null;
            if (dto.EmployeeGroup != null && dto.EmployeeGroup.Id.HasValue)
                empGroup = await EmpGroupRepo.FindAsync(dto.EmployeeGroup.Id.Value, true);

            if (empGroup != null)
                user.EmployeeGroup = empGroup;
        }

        protected void CheckIfEntityHasId(string name, EntityDto dto)
        {
            if (dto == null || dto.HasNoId)
                BadRequest("{0} must be specified.".FormatWith(name));
        }

        protected void CheckIfEntityHasName(string name, NamedEntityDto dto)
        {
            if (dto == null || dto.HasNoName)
                BadRequest("{0} must be specified.".FormatWith(name));
        }

        async Task<Role> GetRole(UserDto dto)
        {
            Role role = null;
            if (dto.Role != null && dto.Role.Id.HasValue)
                role = await RoleRepo.FindAsync(dto.Role.Id.Value, true);
            else
                role = await RoleRepo.FirstOrDefaultAsync(o => o.Name == AppConstants.RoleNames.Employee, true); // RoleRepo.FindByNameAsync(AppConstants.RoleNames.Employee)
            return role;
        }

        async Task CheckIfEmailIsTaken(UserDto dto)
        {
            bool exists = (await UserManager.FindByEmailAsync(dto.Email)) != null;
            if (exists)
                BadRequest("A user account with this email already exists.");
        }

        async Task CheckIfUserNameIsTaken(UserDto dto)
        {
            //bool exists = (await UserManager.FindByNameAsync(dto.UserName) != null);
            bool exists = await Repo.AnyAsync(o => o.UserName.ToUpper() == dto.UserName.ToUpper());
            if (exists)
                BadRequest("A user account with this email already exists.");
        }

        async Task UpdateEmployeeGroup(User user, UserDto dto)
        {
            // if group is not set in dto
            if (dto.EmployeeGroup == null || dto.EmployeeGroup.HasNoId)
            {
                // remove the user from it's current group if it has one
                if (user.EmployeeGroup != null && user.EmployeeGroup.Employees.Contains(user))
                {
                    user.EmployeeGroup.Employees.Remove(user);
                    user.EmployeeGroup = null;
                }
            }
            // if group is set in dto
            if (dto.EmployeeGroup != null && dto.EmployeeGroup.HasId)
            {
                EmployeeGroup newEmpGroup = await EmpGroupRepo.FindAsync(dto.EmployeeGroup.Id.Value, true);
                if (newEmpGroup != null && (user.EmployeeGroup == null || user.EmployeeGroup.Id != newEmpGroup.Id))
                {
                    newEmpGroup.Employees.Add(user);
                    user.EmployeeGroup = newEmpGroup;
                }
            }
        }

        public async Task ActivateUser(Guid id)
        {
            User user = await Repo.FindAsync(id);
            if (user == null) NotFound();

            user.Activate();

            LoggedInUser.AddLog(Enums.ActivityType.Activated, user);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateUser(Guid id)
        {
            User user = await Repo.FindAsync(id);
            if (user == null) NotFound();

            if (user.UserName.ToLower() == "superadmin")
                BadRequest("The default super admin user cannot be deactivated.");

            user.Deactivate();

            LoggedInUser.AddLog(Enums.ActivityType.Deactivated, user);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid id)
        {
            User user = await Repo.FindAsync(id);
            if (user == null) NotFound();

            if (user.UserName.ToLower() == "superadmin")
                BadRequest("The default super admin user cannot be deleted.");

            //if (user.HasChildRecords())
            //    BadRequest("The user cannot be deleted. It has generated some child records on the system. Please deactivate the user if it is not required anymore.");

                //MessageRepo.Delete(user);
                user.Delete();

            LoggedInUser.AddLog(Enums.ActivityType.Deleted, user);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<List<UserNotificationDto>> GetUserNotifications()
        {
            List<UserNotificationDto> result = new List<UserNotificationDto>();

            FindMyNewNotificationsQuery notificationsQuery = new FindMyNewNotificationsQuery();
            notificationsQuery.User = LoggedInUser;
            ISearchResult<Notification> notificationsSearchResult = await NotificationRepo.Find(notificationsQuery);
            ListResult<Notification, UserNotificationDto> notificationsResults = new ListResult<Notification, UserNotificationDto>(notificationsQuery, notificationsSearchResult);
            result.AddRange(notificationsResults.Items);

            FindMyNewMessagesQuery messagesQuery = new FindMyNewMessagesQuery();
            messagesQuery.User = LoggedInUser;
            ISearchResult<Message> messagesSearchResult = await MessageRepo.Find(messagesQuery);
            ListResult<Message, UserNotificationDto> messagesResults = new ListResult<Message, UserNotificationDto>(messagesQuery, messagesSearchResult);
            result.AddRange(messagesResults.Items);

            result.Sort((a, b) =>
            {
                return a.SentAt.CompareTo(b.SentAt);
            });

            return result;
        }
    }
}
