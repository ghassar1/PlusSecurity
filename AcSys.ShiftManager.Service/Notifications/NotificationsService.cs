using AcSys.Core.Data.Model.Base;
using AcSys.Core.Data.Querying;
using AcSys.Core.Email;
using AcSys.Core.Extensions;
using AcSys.Core.ObjectMapping;
using AcSys.ShiftManager.Data.EF.Identity;
using AcSys.ShiftManager.Data.Notifications;
using AcSys.ShiftManager.Data.UnitOfWork;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Model.Helpers;
using AcSys.ShiftManager.Service.Base;
using AcSys.ShiftManager.Service.Results;
using AcSys.ShiftManager.Service.Users;
using Autofac.Extras.NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace AcSys.ShiftManager.Service.Notifications
{
    public class NotificationsService : ApplicationServiceBase, INotificationsService
    {
        #region Public Constructors

        public NotificationsService(
            IUnitOfWork unitOfWork,
            ApplicationRoleManager roleManager,
            ApplicationUserManager userManager,
            INotificationRepository repo,
            INotificationViewRepository noticeViewRepo,
            ILogger logger,
            IEmailService emailService
            ) : base(unitOfWork, roleManager, userManager, logger, emailService)

        {
            //_db = db;
            Repo = repo;
            NotificationViewRepo = noticeViewRepo;

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

        #endregion Public Constructors

        #region Private Properties

        INotificationViewRepository NotificationViewRepo { get; set; }
        INotificationRepository Repo { get; set; }

        #endregion Private Properties

        #region Public Methods

        public async Task<Guid> CreateNotification(NotificationDto dto)
        {
            //if (!ModelState.IsValid) return BadRequest(ModelState);

            Notification notification = ObjectMapper.Map<NotificationDto, Notification>(dto);
            notification.Type = Enums.NotificationType.UserNotification;
            notification.SentAt = DateTime.Now;

            notification.Sender = LoggedInUser;

            notification.Recipients.Clear();
            if (dto.Recipients != null && dto.Recipients.Count > 0)
            {
                foreach (string roleName in dto.Recipients)
                {
                    Role role = await RoleManager.FindByNameAsync(roleName);
                    if (role == null) BadRequest("Invalid role name specified.");

                    //notification.Recipients.Add(role);
                    IEnumerable<User> roleUsers = role.UserRoles.Select(o => o.User);
                    foreach (User user in roleUsers)
                    {
                        notification.Recipients.Add(user);
                    }
                }
            }
            
            if (notification.Recipients.Count == 0)
                notification.IsPublic = true;

            Repo.Add(notification);
            await UnitOfWork.SaveChangesAsync();

            LoggedInUser.AddLog(Enums.ActivityType.Created, notification);
            await UnitOfWork.SaveChangesAsync();

            //return CreatedAtRoute("DefaultApi", new { id = notification.Id }, notification);
            return notification.Id;
        }

        public async Task DeleteNotification(Guid id)
        {
            Notification notification = await Repo.FindAsync(id);
            if (notification == null) NotFound();

            //MessageRepo.Delete(notification);
            notification.Delete();

            LoggedInUser.AddLog(Enums.ActivityType.Deleted, notification);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<IListResult<Notification, NotificationDto>> GetNotifications(FindNotificationsQuery query)
        {
            if (query == null) query = new FindNotificationsQuery();

            ISearchResult<Notification> searchResults = await Repo.Find(query);
            IListResult<Notification, NotificationDto> result = new ListResult<Notification, NotificationDto>(query, searchResults,
                (notification, dto) =>
                {
                    NotificationView view = notification.Views.FirstOrDefault(o => o.User.Id == LoggedInUser.Id);
                    if (view == null)
                    {
                        dto.IsViewed = false;
                        dto.ViewedAt = null;
                    }
                    else
                    {
                        dto.IsViewed = true;
                        dto.ViewedAt = view.ViewedAt;
                    }
                });
            return result;
        }

        public async Task<List<NotificationDto>> GetMyNewNotifications()
        {
            var notifications = await Repo.FindAsync(o => ((o.IsPublic || o.Recipients.Any(r => r.Id == LoggedInUser.Id))
                && !o.Views.Any(u => u.User.Id == LoggedInUser.Id)), o => o.SentAt, true);

            List<NotificationDto> dtos = ObjectMapper.Map<List<Notification>, List<NotificationDto>>(notifications);
            return dtos;
        }

        public async Task<IListResult<Notification, NotificationDto>> GetMyNotifications(FindMyNotificationsQuery query)
        {
            if (query == null) query = new FindMyNotificationsQuery();

            query.UserId = LoggedInUser.Id;
            ISearchResult<Notification> searchResults = await Repo.Find(query);

            IListResult<Notification, NotificationDto> result = new ListResult<Notification, NotificationDto>(query, searchResults, (notification, dto) =>
            {
                //if (notification.Sender != null && notification.Sender.Id == LoggedInUser.Id)
                //{
                //    dto.IsViewed = true;
                //    dto.ViewedAt = notification.SentAt;
                //}

                NotificationView view = notification.Views.FirstOrDefault(o => o.User.Id == LoggedInUser.Id);
                if (view == null)
                {
                    dto.IsViewed = false;
                    dto.ViewedAt = null;
                }
                else
                {
                    dto.IsViewed = true;
                    dto.ViewedAt = view.ViewedAt;
                }
            });

            LoggedInUser.AddLog(Enums.SubjectType.Notification,
                query.SearchCriteria.IsNullOrWhiteSpace() ? Enums.ActivityType.Viewed : Enums.ActivityType.Searched,
                query.SearchCriteria.IsNullOrWhiteSpace() ? string.Empty : "Searched for: {0}".FormatWith(query.SearchCriteria));

            await UnitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<NotificationDto> GetNotification(Guid id)
        {
            Notification notification = await Repo.FindAsync(id);
            if (notification == null) NotFound();

            // check if logged in user can view the notfication
            CanUserViewNotification(LoggedInUser, notification);

            NotificationDto dto = ObjectMapper.Map<Notification, NotificationDto>(notification);

            LoggedInUser.AddLog(Enums.ActivityType.Viewed, notification);
            await UnitOfWork.SaveChangesAsync();

            return dto;
        }

        public async Task<List<UserBasicDetailsDto>> GetRecipients()
        {
            var users = await UserManager.Users.ToListAsync();
            var userDtos = ObjectMapper.Map<List<User>, List<UserBasicDetailsDto>>(users);
            return userDtos;
        }

        public async Task MarkAsRead(Guid id)
        {
            Notification notification = await Repo.FindAsync(id);
            if (notification == null) NotFound();

            CanUserViewNotification(LoggedInUser, notification);

            var viewed = await NotificationViewRepo.AnyAsync(o => o.Notification.Id == id && o.User.Id == LoggedInUser.Id);
            if (viewed) return;

            NotificationView view = new NotificationView()
            {
                Notification = notification,
                User = LoggedInUser,
                ViewedAt = DateTime.Now,
            };
            NotificationViewRepo.Add(view);

            LoggedInUser.AddLog(Enums.ActivityType.Read, notification);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsUnread(Guid id)
        {
            Notification notification = await Repo.FindAsync(id);
            if (notification == null) NotFound();

            CanUserViewNotification(LoggedInUser, notification);

            CanUserMarkAsUnread(LoggedInUser, notification);

            List<NotificationView> views = notification.Views.Where(o => o.User.Id == LoggedInUser.Id).ToList();
            if (views != null && views.Count > 0)
            {
                NotificationViewRepo.Delete(views);

                LoggedInUser.AddLog(Enums.ActivityType.MarkedAsUnread, notification);

                await UnitOfWork.SaveChangesAsync();
            }
        }

        void CanUserMarkAsUnread(User user, Notification notification)
        {
            if (!notification.IsPublic && notification.Recipients != null && notification.Recipients.Count > 0
                            && notification.Sender != null && notification.Sender.Id != user.Id
                            && !notification.Recipients.Any(r => r.Id == user.Id))
                BadRequest("Cannot mark notification as unread. The notification does not belong to you.");
        }

        public async Task UpdateNotification(Guid id, NotificationDto dto)
        {
            //if (id != dto.Id) return BadRequest();

            Notification notification = await Repo.FindAsync(id);
            if (notification == null) NotFound();

            LoggedInUserShouldBe(notification.Sender);

            ObjectMapper.Map<NotificationDto, Notification>(dto, notification);

            notification.Recipients.Clear();
            if (dto.Recipients != null && dto.Recipients.Count > 0)
            {
                foreach (string roleName in dto.Recipients)
                {
                    Role role = await RoleManager.FindByNameAsync(roleName);
                    if (role == null) BadRequest("Invalid role name specified.");

                    //recipients.Add(role);
                    IEnumerable<User> roleUsers = role.UserRoles.Select(o => o.User);
                    foreach (User user in roleUsers)
                    {
                        notification.Recipients.Add(user);
                    }
                }
            }
            
            if (notification.Recipients.Count == 0)
                notification.IsPublic = true;

            Repo.Update(notification);

            LoggedInUser.AddLog(Enums.ActivityType.Updated, notification);

            try
            {
                await UnitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await NotificationExists(id)))
                {
                    NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        void CanUserViewNotification(User user, Notification notification)
        {
            if (!notification.IsPublic
                && notification.Sender != null && notification.Sender.Id != user.Id
                && notification.Recipients != null && notification.Recipients.Count > 0
                //&& !notification.Recipients.Any(r => user.UserRoles.Any(ur => ur.RoleId == r.Id)))
                && !notification.Recipients.Any(r => r.Id == user.Id))
                Unauthorized();
        }
        async Task<bool> NotificationExists(Guid id)
        {
            //return db.Notifications.Count(e => e.Id == id) > 0;
            return (await Repo.CountAsync(e => e.Id == id)) > 0;
        }

        #endregion Private Methods
    }
}
