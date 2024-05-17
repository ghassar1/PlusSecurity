using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcSys.Core.Data.Model.Base;
using AcSys.Core.Data.Querying;
using AcSys.Core.Email;
using AcSys.Core.Extensions;
using AcSys.Core.ObjectMapping;
using AcSys.ShiftManager.Data.EF.Identity;
using AcSys.ShiftManager.Data.Messages;
using AcSys.ShiftManager.Data.Notifications;
using AcSys.ShiftManager.Data.Shifts;
using AcSys.ShiftManager.Data.UnitOfWork;
using AcSys.ShiftManager.Data.Users;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Model.Helpers;
using AcSys.ShiftManager.Service.Base;
using AcSys.ShiftManager.Service.Common;
using AcSys.ShiftManager.Service.Messages;
using AcSys.ShiftManager.Service.Results;
using Autofac.Extras.NLog;

namespace AcSys.ShiftManager.Service.Shifts
{
    public class ShiftsService : ApplicationServiceBase, IShiftsService
    {
        IShiftRepository Repo { get; set; }
        IUserRepository UserRepo { get; set; }
        IEmployeeGroupRepository EmpGroupRepo { get; set; }
        IMessageRepository MessageRepo { get; set; }
        INotificationRepository NotificationRepo { get; set; }

        public ShiftsService(IUnitOfWork unitOfWork,
            ApplicationRoleManager roleManager,
            ApplicationUserManager userManager,
            IShiftRepository repo,
            IUserRepository userRepo,
            IEmployeeGroupRepository empGroupRepo,
            IMessageRepository messageRepo,
            INotificationRepository notificationRepo,
            ILogger logger,
            IEmailService emailService)
             : base(unitOfWork, roleManager, userManager, logger, emailService)
        {
            Repo = repo;
            UserRepo = userRepo;
            EmpGroupRepo = empGroupRepo;
            MessageRepo = messageRepo;
            NotificationRepo = notificationRepo;
        }

        public async Task<RotaDto> Get(FindShiftsQuery query)
        {
            if (query == null)
            {
                query = new FindShiftsQuery()
                {
                    StartDate = DateTime.Today.BeginningOfTheWeek(),
                    EndDate = DateTime.Today.EndOfTheWeek()
                };
            }

            if (LoggedInUserHasAnyRoleIn(AppConstants.RoleNames.Employee))
            {
                if (LoggedInUser.EmployeeGroup == null)
                {
                    query.FilterUnGrouped = true;
                }
                else
                {
                    query.GroupId = LoggedInUser.EmployeeGroup.Id;
                }
            }

            Hashtable rotaItemsHashtable = new Hashtable();
            ISearchResult<User> employeeUsers = await GetEmployeeUsers(query);
            foreach (User user in employeeUsers.Records)
            {
                RotaItemDto rotaItem = new RotaItemDto()
                {
                    Employee = ObjectMapper.Map<User, NamedEntityDto>(user)
                };
                rotaItemsHashtable[user.Id] = rotaItem;
            }

            List<ShiftBasicDetailsDto> openShifts = new List<ShiftBasicDetailsDto>();

            ISearchResult<Shift> searchResults = await Repo.Find(query);
            //ListResult<Shift, ShiftDto> result = new ListResult<Shift, ShiftDto>(query, searchResults);

            foreach (Shift shift in searchResults.Records)
            {
                shift.UpdateStatus();

                ShiftBasicDetailsDto shiftDto = ObjectMapper.Map<Shift, ShiftBasicDetailsDto>(shift);
                if (shift.Employee == null)
                {
                    openShifts.Add(shiftDto);
                }
                else
                {
                    RotaItemDto rotaItem = rotaItemsHashtable[shift.Employee.Id] as RotaItemDto;
                    if (rotaItem == null)
                    {
                        rotaItem = new RotaItemDto()
                        {
                            Employee = ObjectMapper.Map<User, NamedEntityDto>(shift.Employee)
                        };
                        rotaItemsHashtable[shift.Employee.Id] = rotaItem;
                    }
                    rotaItem.Shifts.Add(shiftDto);
                }

            }
            await UnitOfWork.SaveChangesIfAnyAsync();

            RotaDto dto = new RotaDto()
            {
                OpenShifts = openShifts,
                Items = rotaItemsHashtable.Values.Cast<RotaItemDto>().ToList()
            };
            return dto;
        }

        async Task<ISearchResult<User>> GetEmployeeUsers(FindShiftsQuery query)
        {
            //users = await UserRepo.FindAsync(o => o.EntityStatus == EntityStatus.Active && o.EmployeeGroup != null && o.EmployeeGroup.Id == query.GroupId.Value);

            FindUsersQuery usersQuery = new FindUsersQuery() { PageNo = -1, PageSize = -1, IncludeRoles = new string[] { "Employee" } };
            usersQuery.Status = EntityStatus.Active;

            if (query.GroupId.HasValue)
            {
                usersQuery.EmployeeGroupIds = new Guid[] { query.GroupId.Value };
            }

            usersQuery.FilterUsersInNoGroup = query.FilterUnGrouped;

            ISearchResult<User> employeeUsers = await UserRepo.Find(usersQuery);
            return employeeUsers;
        }

        public async Task<ShiftDto> Get(Guid id)
        {
            Shift shift = await Repo.FindAsync(id, true);
            shift.UpdateStatus();
            await UnitOfWork.SaveChangesIfAnyAsync();
            ShiftDto dto = ObjectMapper.Map<Shift, ShiftDto>(shift);
            dto.DateTimeOffset = DateTimeOffset.Now;
            dto.Now = DateTime.Now;
            dto.NowUtc = DateTime.UtcNow;
            dto.TimeZone = TimeZone.CurrentTimeZone;
            return dto;
        }

        public async Task<List<Guid>> Create(CreateShiftDto dto)
        {
            List<Shift> shifts = new List<Shift>();

            CheckIfPropertyHasValue("Start Date", dto.StartDate);
            CheckIfPropertyHasValue("End Date", dto.EndDate);

            List<User> employees = await GetEmployeesList(dto);
            StringBuilder shiftsLinesBuilder = new StringBuilder();
            
            var dates = dto.StartDate.Value.Date.GetDateRangeInclusiveTo(dto.EndDate.Value.Date);
            foreach (DateTime date in dates)
            {
                if (dto.Days != null && dto.Days.Count() > 0 && !dto.Days.Contains((int)date.DayOfWeek))
                    continue;

                if (employees.Count == 0)
                {
                    if (!dto.ShiftsPerDay.HasValue && dto.ShiftsPerDay.Value < 1)
                        dto.ShiftsPerDay = 1;

                    for (int i = 0; i < dto.ShiftsPerDay.Value; i++)
                    {
                        Shift shift = await CreateShift(date, dto);

                        shiftsLinesBuilder.AppendLine("{0} on '{1}' from '{2}' to '{3}'."
                        .FormatWith(shift.Title, shift.StartTime.ToFormattedDateString(),
                            shift.StartTime.ToShortFormattedTimeString(), shift.EndTime.ToShortFormattedTimeString()));

                        shifts.Add(shift);
                    }
                }
                else
                {
                    foreach (User employee in employees)
                    {
                        Shift shift = await CreateShift(date, dto, employee);
                        shifts.Add(shift);
                    }
                }
            }

            if (employees.Count == 0)
            {
                await CreateNotificationForOpenShifts(shifts, shiftsLinesBuilder);
            }
            else
            {
                foreach (User employee in employees)
                {
                    CreateMessageForEmployeeShift(shifts, shiftsLinesBuilder, employee);
                }
            }

            await UnitOfWork.SaveChangesAsync();

            //LoggedInUser.AddLog(Enums.SubjectType.Shift, Enums.ActivityType.Created, shift.ToDescription(), shift);
            //await UnitOfWork.SaveChangesAsync();

            return shifts.Select(o => o.Id).ToList();
        }

        void CreateMessageForEmployeeShift(List<Shift> shifts, StringBuilder shiftsLinesBuilder, User employee)
        {
            string subject = shifts.Count == 1 ?
                                    "New Open Shift on {0}".FormatWith(shifts.FirstOrDefault().StartTime.ToFormattedDateString()) :
                                    "New Open Shifts from {0} to {1}".FormatWith(shifts.FirstOrDefault().StartTime.ToFormattedDateString(), shifts.LastOrDefault().StartTime.ToFormattedDateString());

            string messageText = PrepareMessageText(shifts, shiftsLinesBuilder, employee);

            Message message = new Message()
            {
                Sender = LoggedInUser,
                SentAt = DateTime.Now,
                Subject = subject,
                Text = messageText,
            };
            message.Recipients.Add(employee);
            employee.IncomingMessages.Add(message);

            MessageRepo.Add(message);
        }

        string PrepareMessageText(List<Shift> shifts, StringBuilder shiftsLinesBuilder, User employee)
        {
            //var employeeShifts = shifts.Where(o => o.Employee != null && o.Employee.Id == employee.Id);
            foreach (Shift shift in shifts)
            {
                if (shift.Employee != null && shift.Employee.Id == employee.Id)
                {
                    shiftsLinesBuilder.AppendLine("{0} on '{1}' from '{2}' to '{3}'."
                        .FormatWith(shift.Title, shift.StartTime.ToFormattedDateString(),
                        shift.StartTime.ToShortFormattedTimeString(), shift.EndTime.ToShortFormattedTimeString()));
                }
            }

            StringBuilder messageTextBuilder = new StringBuilder();

            messageTextBuilder.AppendLine("Hi {0}".FormatWith(employee.FirstName));
            messageTextBuilder.AppendLine("");

            messageTextBuilder.AppendLine(shifts.Count == 1 ?
                "You have been allocated to a new shift." :
                "You have been allocated to new shifts.");

            messageTextBuilder.AppendLine("");
            messageTextBuilder.AppendLine(shiftsLinesBuilder.ToString());
            messageTextBuilder.AppendLine("");
            messageTextBuilder.AppendLine("");
            messageTextBuilder.AppendLine(shifts.Count == 1 ?
                "You are advised to arrive on time on the day." :
                "You are advised to arrive on time on these days.");
            messageTextBuilder.AppendLine("");
            messageTextBuilder.AppendLine("Regards.");
            messageTextBuilder.AppendLine("");
            messageTextBuilder.AppendLine("{0}".FormatWith(LoggedInUser.GetFullName()));

            string messageText = messageTextBuilder.ToString();
            return messageText;
        }

        async Task CreateNotificationForOpenShifts(List<Shift> shifts, StringBuilder shiftsLinesBuilder)
        {
            string notificationText = PrepareNotificationsText(shifts, shiftsLinesBuilder);
            string title = shifts.Count == 1 ?
                "New Open Shift on {0}".FormatWith(shifts.FirstOrDefault().StartTime.ToFormattedDateString()) :
                "New Open Shifts from {0} to {1}".FormatWith(shifts.FirstOrDefault().StartTime.ToFormattedDateString(), shifts.LastOrDefault().StartTime.ToFormattedDateString());

            Notification notification = new Notification()
            {
                Type = Enums.NotificationType.UserNotification,
                SentAt = DateTime.Now,
                IsPublic = false,
                Sender = LoggedInUser,
                Title = title,
                Text = notificationText
            };
            notification.Sender = LoggedInUser;

            notification.Recipients.Clear();

            Role role = await RoleManager.FindByNameAsync(AppConstants.RoleNames.Employee);
            if (role == null) BadRequest("Invalid role name specified.");

            IEnumerable<User> roleUsers = role.UserRoles.Select(o => o.User);
            foreach (User user in roleUsers)
            {
                notification.Recipients.Add(user);
            }

            NotificationRepo.Add(notification);
        }

        string PrepareNotificationsText(List<Shift> shifts, StringBuilder shiftsLinesBuilder)
        {
            StringBuilder notificationTextBulder = new StringBuilder();

            notificationTextBulder.AppendLine("Hi");
            notificationTextBulder.AppendLine("");

            notificationTextBulder.AppendLine(shifts.Count == 1 ?
                "New open shift has been created." :
                "New open shifts have been created.");

            notificationTextBulder.AppendLine("");
            notificationTextBulder.AppendLine(shiftsLinesBuilder.ToString());
            notificationTextBulder.AppendLine("");
            notificationTextBulder.AppendLine(shifts.Count == 1 ?
                "You are requested to consider the shift if you can take it." :
                "You are requested to consider the shifts if you can take any.");
            notificationTextBulder.AppendLine("");
            notificationTextBulder.AppendLine("Regards.");
            notificationTextBulder.AppendLine("");
            notificationTextBulder.AppendLine("{0}".FormatWith(LoggedInUser.GetFullName()));

            string notificationText = notificationTextBulder.ToString();
            return notificationText;
        }

        //public async Task EmailNewOpenShift(User user, string roles)
        //{
        //    string activationURLBase = GetBaseURL();
        //    string url = "{0}#/Login".FormatWith(activationURLBase);

        //    string body = @"You have been invited on Accountsware to the organisation '{0}' with the role/s '{1}'. <br /><br /> 
        //                    Please log into your account on Accountsware and your will find the organisation in your Dashboard.
        //                    <br /><br />Click <a href='{2}'>here</a> to log in. <br /><br />The Accountsware Team."
        //        .FormatWith(roles, url);

        //    await EmailService.SendAsync(user.Email, "Invitation to the Organisation", body);
        //}

        async Task<Shift> CreateShift(DateTime date, CreateShiftDto dto, User employee = null)
        {
            //Shift shift = ObjectMapper.Map<CreateShiftDto, Shift>(dto);
            Shift shift = new Shift()
            {
                Employee = employee,
                IsOpen = employee == null,

                //StartTime = startTime,
                //EndTime = endTime,
                TotalBreakMins = dto.TotalBreakMins,

                Title = dto.Title,
                Notes = dto.Notes,

                ClockInTime = null,
                ClockOutTime = null
            };

            await SetStartAndEndTimes(shift, date, dto.StartTime.Value, dto.EndTime.Value);
            
            Repo.Add(shift);

            LoggedInUser.AddLog(Enums.ActivityType.Created, shift);

            return shift;
        }

        private static DateTime CreateDateTime(DateTime date, TimeSpan ts)
        {
            return new DateTime(date.Year, date.Month, date.Day, ts.Hours, ts.Minutes, 0);
        }

        async Task<List<User>> GetEmployeesList(CreateShiftDto dto)
        {
            List<User> employees = new List<User>();

            foreach (var groupDto in dto.Groups)
            {
                EmployeeGroup employeeGroup = null;
                if (groupDto != null && groupDto.HasId)
                {
                    employeeGroup = await EmpGroupRepo.FindAsync(groupDto.Id.Value);

                    foreach (User user in employeeGroup.Employees)
                    {
                        employees.Add(user);
                    }
                }
            }

            foreach (var empDto in dto.Employees)
            {
                if (empDto != null && empDto.HasId)
                {
                    User user = await UserRepo.FindAsync(empDto.Id.Value);
                    employees.Add(user);
                }
            }

            return employees;
        }

        public async Task Update(Guid id, UpdateShiftDto dto)
        {
            Shift shift = await Repo.FindAsync(id, true);

            if (shift.HasClockedOut())
                BadRequest("Cannot update shift. The employee has already clocked out of the shift.");

            if (shift.HasClockedIn())
                BadRequest("Cannot update shift. The employee has already clocked into the shift.");

            if (!shift.HasReasonableTimeInStart())
                BadRequest("Shift cannot be updated after or shortly before it's start.");

            if (dto.Employee != null && dto.Employee.HasId)
            {
                if (shift.Employee == null || shift.Employee.Id != dto.Employee.Id.Value)
                {
                    User employee = await UserRepo.FindAsync(dto.Employee.Id.Value);
                    shift.Employee = employee;
                    shift.IsOpen = false;
                }
            }
            else
            {
                shift.Employee = null;
            }

            await SetStartAndEndTimes(shift, dto.Date.Value, dto.StartTime.Value, dto.EndTime.Value);
            
            //ObjectMapper.Map<Shift, UpdateShiftDto>(shift, dto);

            shift.IsOpen = shift.Employee == null;

            shift.TotalBreakMins = dto.TotalBreakMins;
            shift.Title = dto.Title;
            shift.Notes = dto.Notes;

            shift.ClockInTime = null;
            shift.ClockOutTime = null;

            shift.UpdateStatus();

            Repo.Update(shift);

            LoggedInUser.AddLog(Enums.ActivityType.Updated, shift);

            await UnitOfWork.SaveChangesAsync();
        }

        async Task<bool> AnyShiftOverlaps(Shift shift, User employee, DateTime start, DateTime end)
        {
            FindShiftsQuery query = new FindShiftsQuery()
            {
                CompareExactDateTime = true,
                StartDate = start,
                EndDate = end
            };

            if (employee != null)
            {
                query.IncludeEmployeeIds.Add(employee.Id);
            }

            query.ExcludeShiftIds.Add(shift.Id);

            int count = await Repo.Count(query);
            bool overlap = count > 0;
            return overlap;
        }

        public async Task Assign(Guid id, AssignShiftDto dto)
        {
            Shift shift = await Repo.FindAsync(id, true);

            if (shift.HasClockedOut())
                BadRequest("Cannot update shift. The employee has already clocked out of the shift.");

            if (shift.HasClockedIn())
                BadRequest("Cannot update shift. The employee has already clocked into the shift.");

            //if (!shift.HasReasonableTimeInStart())
            //    BadRequest("Shift cannot be updated after or shortly before it's start.");

            if (dto.IsOpen)
            {
                shift.IsOpen = true;

                if (shift.Employee != null)
                {
                    shift.Employee.Shifts.Remove(shift);
                    shift.Employee = null;
                }
            }
            else if (dto.EmployeeId.HasValue)
            {
                if (shift.Employee == null || shift.Employee.Id != dto.EmployeeId.Value)
                {
                    User employee = await UserRepo.FindAsync(dto.EmployeeId.Value);
                    shift.Employee = employee;
                    shift.IsOpen = false;
                }
            }

            await SetStartAndEndTimes(shift, dto.Start.Value, dto.Start.Value.TimeOfDay, dto.End.Value.TimeOfDay);
            
            shift.ClockInTime = null;
            shift.ClockOutTime = null;

            shift.UpdateStatus();

            Repo.Update(shift);

            LoggedInUser.AddLog(Enums.ActivityType.Updated, shift);

            await UnitOfWork.SaveChangesAsync();
        }

        async Task SetStartAndEndTimes(Shift shift, DateTime date, TimeSpan start, TimeSpan end)
        {
            DateTime startTime = CreateDateTime(date, start);
            DateTime endTime = CreateDateTime(date, end);
            if (endTime.IsBefore(startTime))
                endTime = endTime.AddDays(1);

            if (shift.Employee != null)
            {
                bool overlap = await AnyShiftOverlaps(shift, shift.Employee, startTime, endTime);
                if (overlap)
                    BadRequest("The employee has an overlapping shift.");
            }

            shift.StartTime = startTime;
            shift.EndTime = endTime;
        }

        public async Task Take(Guid id)
        {
            Shift shift = await Repo.FindAsync(id, true);

            if(!shift.IsOpen)
                BadRequest("Shift has already been taken.");

            bool overlap = await AnyShiftOverlaps(shift, shift.Employee, shift.StartTime, shift.EndTime);
            if (overlap)
                BadRequest("You cannot take this shift. You have an overlapping shift.");

            shift.Take(LoggedInUser);

            LoggedInUser.AddLog(Enums.ActivityType.ShiftTaken, shift);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task Leave(Guid id)
        {
            Shift shift = await Repo.FindAsync(id, true);

            if (shift.IsOpen)
                BadRequest("Shift is already open for allocation.");

            shift.Leave();

            LoggedInUser.AddLog(Enums.ActivityType.ShiftLeft, shift);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task ClockIn(Guid id)
        {
            Shift shift = await Repo.FindAsync(id, true);

            if (shift.HasClockedIn())
                BadRequest("Shift has already been clocked into.");

            //if (DateTime.Now.DateDiff("minute", shift.StartTime))
            //    BadRequest("Shift cannot be clocked in earlier than it starts.");

            if (shift.NotStartingShortly())
                BadRequest("Shift can only be clocked in 15 mins before start. [{0}]".FormatWith(DateTime.Now));

            DateTime now = DateTime.Now;
            //if (now.DateDiff("minute", shift.EndTime) < 60)
            //    throw new ApplicationException("Shift can not be clocked in during the last 60 mins.");

            shift.ClockIn(now);

            LoggedInUser.AddLog(Enums.ActivityType.ClockIn, shift);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task ClockOut(Guid id)
        {
            DateTime now = DateTime.Now;

            Shift shift = await Repo.FindAsync(id, true);

            if (shift.HasClockedOut())
                BadRequest("Shift has already been clocked out of.");

            if (shift.HasNotClockedIn())
                BadRequest("Shift has not been clocked into yet.");

            //if (shift.NotStartingShortly())
            //    BadRequest("Shift can not be clocked out before it's end.");

            if (now.IsBefore(shift.StartTime))
                throw new ApplicationException("Shift clock out time cannot be earlier than it's start.");

            //if (shift.StartTime.DateDiff("minute", now) <= 30)
            //    throw new ApplicationException("Shift can not be clocked out during the first 30 mins.");

            shift.ClockOut(now);

            LoggedInUser.AddLog(Enums.ActivityType.ClockOut, shift);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            Shift shift = await Repo.FindAsync(id, true);
            shift.UpdateStatus();

            if (false && shift.Status != Enums.ShiftStatus.Due)
                BadRequest("Cannot delete completed or missed shifts.");

            if (false && !shift.HasReasonableTimeInStart())
                BadRequest("Shift cannot be deleted shortly before or after it's start.");

            //if (shift.HasChildRecords())
            //    BadRequest("Shift cannot be deleted.");

            Repo.Delete(shift);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(List<Guid> ids)
        {
            Repo.Delete(ids);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<List<AttendanceReportRowDto>> GetAttendanceData(AttendanceReportShiftsQuery query)
        {
            if (query == null)
            {
                query = new AttendanceReportShiftsQuery()
                {
                    StartDate = DateTime.Today.BeginningOfTheWeek(),
                    EndDate = DateTime.Today.EndOfTheWeek()
                };
            }

            List<AttendanceReportRowDto> dtos = new List<AttendanceReportRowDto>();

            ISearchResult<Shift> searchResults = await Repo.Find(query);
            foreach (Shift shift in searchResults.Records)
            {
                shift.UpdateStatus();

                AttendanceReportRowDto dto = ObjectMapper.Map<Shift, AttendanceReportRowDto>(shift);
                dtos.Add(dto);
            }
            await UnitOfWork.SaveChangesIfAnyAsync();

            dtos = dtos.OrderBy(o => o.EmployeeName).ToList();

            string name = string.Empty;
            foreach (var dto in dtos)
            {
                if (dto.EmployeeName == name)
                    dto.EmployeeName = "";
                else
                    name = dto.EmployeeName;
            }
            return dtos;
        }

        public async Task<AttendanceSummaryReportDto> GetAttendanceSummaryData(AttendanceReportShiftsQuery query)
        {
            if (query == null)
            {
                query = new AttendanceReportShiftsQuery()
                {
                    StartDate = DateTime.Today.BeginningOfTheWeek(),
                    EndDate = DateTime.Today.EndOfTheWeek()
                };
            }

            AttendanceSummaryReportDto dto = new AttendanceSummaryReportDto();
            dto.PeriodStart = query.StartDate.Value;
            dto.PeriodEnd = query.EndDate.Value;

            ISearchResult<Shift> searchResults = await Repo.Find(query);

            searchResults.Records.ForEach(o => o.UpdateStatus());
            await UnitOfWork.SaveChangesAsync();

            dto.TotalShifts = searchResults.TotalRecords;
            dto.DueShifts = searchResults.Records.Count(o => o.Status == Enums.ShiftStatus.Due);
            dto.CompletedShifts = searchResults.Records.Count(o => o.Status == Enums.ShiftStatus.Complete);
            dto.LateShifts = searchResults.Records.Count(o => o.StartStatus == Enums.ShiftStartStatus.Late);

            if (dto.TotalShifts > 0)
            {
                dto.TotalEmployees = searchResults.Records.GroupBy(o => o.Employee).ToList().Count();

                dto.TotalHours = searchResults.Records.Sum(o => o.TotalMins);
                dto.TotalHours = dto.TotalHours / 60;

                dto.TotalClockedHours = (int)Math.Floor((double)searchResults.Records.Sum(o => o.ClockedMins) / 60);

                dto.AttendancePercentage = (int)((double)dto.CompletedShifts / dto.TotalShifts * 100);

                dto.LateAttendancePercentage = (int)((double)dto.LateShifts / dto.TotalShifts * 100);
            }

            return dto;
        }

        public async Task<DashboardDto> GetDashboardData()
        {
            Guid? employeeId = null;
            if (LoggedInUserHasAnyRoleIn(AppConstants.RoleNames.Employee))
                employeeId = LoggedInUser.Id;

            DashboardDto dto = new DashboardDto();

            DateTime start = DateTime.Today.BeginningOfTheWeek();
            DateTime end = DateTime.Today.EndOfTheWeek();

            dto.CurrentWeekStats = await GetPeriodStats(employeeId, start, end);

            start = DateTime.Today.BeginningOfTheMonth();
            end = DateTime.Today.EndOfTheMonth();

            dto.CurrentMonthStats = await GetPeriodStats(employeeId, start, end);

            start = start.BeginningOfLastMonth();
            end = end.EndOfLastMonth();

            dto.LastMonthStats = await GetPeriodStats(employeeId, start, end);

            await SetPerformanceChartData(dto, employeeId);

            dto.Messages = await GetMessages();

            return dto;
        }

        async Task<List<MessageDto>> GetMessages()
        {
            FindMyInBoxMessagesQuery query = new FindMyInBoxMessagesQuery()
            {
                PageSize = 5, PageNo=1
            };
            query.UserId = LoggedInUser.Id;

            ISearchResult<Message> messages = await MessageRepo.Find(query);

            IListResult<Message, MessageDto> results = new ListResult<Message, MessageDto>(query, messages,
                (message, messageDto) =>
                {
                    MessageView view = message.Views.FirstOrDefault(o => o.User.Id == LoggedInUser.Id);
                    if (view == null)
                    {
                        messageDto.IsViewed = false;
                        messageDto.ViewedAt = null;
                    }
                    else
                    {
                        messageDto.IsViewed = true;
                        messageDto.ViewedAt = view.ViewedAt;
                    }
                });
            return results.Items;
        }

        async Task<ShiftStatsDto> GetPeriodStats(Guid? employeeId, DateTime start, DateTime end)
        {
            ShiftStatsDto stats = new ShiftStatsDto();

            //totalShiftsCount = await GetShiftsCount(start, end, employeeId);
            ISearchResult<Shift> searchResults = await GetShifts(start, end, employeeId);

            searchResults.Records.ForEach(o => o.UpdateStatus());
            await UnitOfWork.SaveChangesAsync();

            stats.Total = searchResults.TotalRecords;
            if (stats.Total > 0)
            {
                //stats.Completed = await GetShiftsCount(start, end, employeeId, Enums.ShiftStatus.Complete);
                //stats.Missed = await GetShiftsCount(start, end, employeeId, Enums.ShiftStatus.Missed);
                //stats.Due = await GetShiftsCount(start, end, employeeId, Enums.ShiftStatus.Due);
                //stats.OnTime = await GetShiftsCount(start, end, employeeId, Enums.ShiftStatus.Complete, Enums.ShiftStartStatus.OnTime);
                //stats.Late = await GetShiftsCount(start, end, employeeId, Enums.ShiftStatus.Complete, Enums.ShiftStartStatus.Late);

                stats.TotalHours = (int)Math.Floor((double)searchResults.Records.Sum(o => o.TotalMins) / 60);
                stats.TotalClockedHours = (int)Math.Floor((double)searchResults.Records.Sum(o => o.ClockedMins) / 60);

                stats.Completed = searchResults.Records.Count(o => o.Status == Enums.ShiftStatus.Complete);
                stats.Missed = searchResults.Records.Count(o => o.Status == Enums.ShiftStatus.Missed);
                stats.Due = searchResults.Records.Count(o => o.Status == Enums.ShiftStatus.Due);
                stats.OnTime = searchResults.Records.Count(o => o.Status == Enums.ShiftStatus.Complete && o.StartStatus == Enums.ShiftStartStatus.OnTime);
                stats.Late = searchResults.Records.Count(o => o.Status == Enums.ShiftStatus.Complete && o.StartStatus == Enums.ShiftStartStatus.Late);

                stats.CompletedPercentage = Math.Round((double)stats.Completed / stats.Total * 100, 2, MidpointRounding.AwayFromZero);
                stats.MissedPercentage = Math.Round((double)stats.Missed / stats.Total * 100, 2, MidpointRounding.AwayFromZero);
                stats.DuePercentage = Math.Round((double)stats.Due / stats.Total * 100, 2, MidpointRounding.AwayFromZero);
                stats.OnTimePercentage = Math.Round((double)stats.OnTime / stats.Completed * 100, 2, MidpointRounding.AwayFromZero);
                stats.LatePercentage = Math.Round((double)stats.Late / stats.Completed * 100, 2, MidpointRounding.AwayFromZero);


                //dto.AttendanceThisWeek.Add(new ChartDataItemDto("Completed", stats.CompletedPercentage));
                //dto.AttendanceThisWeek.Add(new ChartDataItemDto("Missed", stats.MissedPercentage));
                //dto.AttendanceThisWeek.Add(new ChartDataItemDto("Due", stats.DuePercentage));

                //dto.LateThisWeek.Add(new ChartDataItemDto("OnTime", stats.OnTimePercentage));
                //dto.LateThisWeek.Add(new ChartDataItemDto("Late", stats.LatePercentage));
            }

            return stats;
        }

        async Task<ISearchResult<Shift>> GetShifts(DateTime start, DateTime end, Guid? employeeId = null, Enums.ShiftStatus? status = null, Enums.ShiftStartStatus? startStatus = null, Enums.ShiftEndStatus? endStatus = null)
        {
            var query = new AttendanceReportShiftsQuery(start, end, employeeId, status, startStatus, endStatus);
            ISearchResult<Shift> searchResults = await Repo.Find(query);
            return searchResults;
        }

        async Task<int> GetShiftsCount(DateTime start, DateTime end, Guid? employeeId = null, Enums.ShiftStatus? status = null, Enums.ShiftStartStatus? startStatus = null, Enums.ShiftEndStatus? endStatus = null)
        {
            var query = new AttendanceReportShiftsQuery(start, end, employeeId, status, startStatus, endStatus);
            int count = await Repo.Count(query);
            return count;
        }

        async Task SetPerformanceChartData(DashboardDto dto, Guid? employeeId)
        {
            DateTime start = DateTime.Today.BeginningOfTheMonth();
            DateTime end = DateTime.Today.EndOfTheMonth();

            for (int i = 0; i < 6; i++)
            {
                PerformanceChartItemDto item = new PerformanceChartItemDto()
                {
                    Label = start.ToString("MMMM"),
                    Stats = await GetPeriodStats(employeeId, start, end)
                };
                dto.PerformanceChartItems.Add(item);

                start = start.BeginningOfLastMonth();
                end = end.EndOfLastMonth();
            }

            dto.PerformanceChartItems.Reverse();
        }
    }
}
