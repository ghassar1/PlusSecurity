namespace AcSys.ShiftManager.Data.EF.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.IO;
    using System.Linq;
    using Context;
    using Core.Data.MsSqlServer;
    using Core.Extensions;
    using Elmah.Contrib.EntityFramework;
    using Microsoft.AspNet.Identity;
    using Model;
    using Model.Helpers;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        PasswordHasher passwordHasher = new PasswordHasher();

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<AwDbContext, Configuration>());
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AwDbContext>());

            try
            {
                //if (!context.Database.CompatibleWithModel(true)) context.Database.Delete();
                //if (context.Database.Exists()) context.Database.Delete();
                //if (!context.Database.Exists()) context.Database.Create();

                CreateElmahTable(context);

                CreateNLogTable(context);

                Role superAdminRole = CreateRole(context, AppConstants.RoleNames.SuperAdmin);
                Role adminRole = CreateRole(context, AppConstants.RoleNames.Admin);
                Role hrManagerRole = CreateRole(context, AppConstants.RoleNames.HRManager);
                Role recManagerRole = CreateRole(context, AppConstants.RoleNames.RecManager);
                Role employeeRole = CreateRole(context, AppConstants.RoleNames.Employee);

                //EmployeeGroup groupOne = CreateEmployeeGroup(context, "Group One");
                //EmployeeGroup groupTwo = CreateEmployeeGroup(context, "Group Two");

                context.SaveChanges();

                User superAdmin = CreateUser(context, superAdminRole, "superadmin", "P@ssw0rd", "Super", "Admin", "superadmin@gms.com", "0764818160", "07646591144", DateTime.Now.AddYears(-35));
                User admin = CreateUser(context, adminRole, "admin", "P@ssw0rd", "Admin", "User", "admin@gms.com", "0763252160", "07643261144", DateTime.Now.AddYears(-30));
                User hrManager = CreateUser(context, hrManagerRole, "hrmanager", "P@ssw0rd", "HR", "Manager", "hrmanager@gms.com", "07632355160", "07632421144", DateTime.Now.AddYears(-32));
                User recManager = CreateUser(context, recManagerRole, "recmanager", "P@ssw0rd", "Recruitement", "Manager", "recmanager@gms.com", "07632352342", "076324223421", DateTime.Now.AddYears(-32));
                //User emp01 = CreateUser(context, employeeRole, "emp01", "P@ssw0rd", "Emp", "One", "emp01@gms.com", "076323234252", "076322342109", DateTime.Now.AddYears(-31));
                //User emp02 = CreateUser(context, employeeRole, "emp02", "P@ssw0rd", "Emp", "Two", "emp02@gms.com", "076323234252", "076322342209", DateTime.Now.AddYears(-32));

                //Notification testNotficationForAdmins = CreateNotification(context, superAdmin, "Test Notification by Super Admin", "Test Notification by Super Admin........", false, superAdminRole, adminRole);
                //Notification testNotficationForAll = CreateNotification(context, admin, "Test Notification by Admin", "Test Notification by Admin for all........", true);
                //Notification testNotficationForManagers = CreateNotification(context, admin, "Test Notification by Admin", "Test Notification by Admin for all........", true, hrManagerRole, recManagerRole);

                //Message testMessage1 = CreateMessage(context, superAdmin, "Test Message by Super Admin", "Test Message by Super Admin........", superAdmin, admin);
                //Message testMessage2 = CreateMessage(context, admin, "Test Message by Admin for All", "Test Message by Admin for all........", superAdmin);
                //Message testMessage3 = CreateMessage(context, admin, "Test Message by Admin for Managers", "Test Message by Admin for Managers........", hrManager, recManager);

                context.SaveChanges();

                //DateTime startDate = DateTime.Now.AddDays(-30);
                //DateTime endDate = DateTime.Now.Date.AddDays(30);
                //IEnumerable<DateTime> dateRange = startDate.GetDateRangeInclusiveTo(endDate);

                //// create allocated shifts
                //foreach (DateTime date in dateRange)
                //{
                //    DateTime startTime = new DateTime(date.Year, date.Month, date.Day, 9, 0, 0, 0);
                //    DateTime endTime = startTime.AddHours(8);

                //    Shift shift1 = CreateShift(context, emp01, startTime, endTime, "9:00 AM - 5:00 PM");
                //    Shift shift2 = CreateShift(context, emp02, startTime, endTime, "9:00 AM - 5:00 PM");

                //    if (date.IsBefore(DateTime.Now))
                //    {
                //        if (date.Day % 5 == 0)
                //        {
                //            shift1.ClockIn(shift1.StartTime);
                //            shift1.ClockOut(shift1.EndTime);

                //            shift2.ClockIn(shift1.StartTime);
                //            shift2.ClockOut(shift1.EndTime);
                //        }
                //        else if (date.Day % 6 == 0)
                //        {
                //            continue;
                //        }
                //        else if (date.Day % 7 == 0)
                //        {
                //            shift1.ClockIn(shift1.StartTime.AddMinutes(10));
                //            shift1.ClockOut(shift1.EndTime);

                //            shift2.ClockIn(shift2.StartTime.AddMinutes(15));
                //            shift2.ClockOut(shift2.EndTime.AddMinutes(-30));
                //        }
                //        else
                //        {
                //            shift1.ClockIn(shift2.StartTime.AddMinutes(-1));
                //            shift1.ClockOut(shift2.EndTime.AddMinutes(1));

                //            shift2.ClockIn(shift2.StartTime.AddMinutes(-1));
                //            shift2.ClockOut(shift2.EndTime.AddMinutes(1));
                //        }
                //    }
                //}

                //// create open shifts
                //foreach (DateTime date in DateTime.Now.GetDateRangeInclusiveTo(DateTime.Now.Date.AddDays(6)))
                //{
                //    DateTime startTime = new DateTime(date.Year, date.Month, date.Day, 17, 0, 0, 0);
                //    DateTime endTime = startTime.AddHours(8);
                //    CreateShift(context, null, startTime, endTime, "5:00 PM - 1:00 AM");
                //    CreateShift(context, null, startTime, endTime, "5:00 PM - 1:00 AM");
                //}

                //context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (DbEntityValidationResult errors in ex.EntityValidationErrors)
                {
                    foreach (DbValidationError e in errors.ValidationErrors)
                    {
                        Console.WriteLine(e.ErrorMessage);
                        throw new ApplicationException(string.Format("{0}: {1}", e.PropertyName, e.ErrorMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        Role CreateRole(ApplicationDbContext context, string roleName)
        {
            Role role = new Role()
            {
                Name = roleName,
            };
            context.Roles.AddOrUpdate(o => o.Name, role);
            return role;
        }

        EmployeeGroup CreateEmployeeGroup(ApplicationDbContext context, string groupName)
        {
            EmployeeGroup group = new EmployeeGroup()
            {
                Name = groupName,
            };
            context.EmployeeGroups.AddOrUpdate(o => o.Name, group);
            return group;
        }

        void CreateElmahTable(ApplicationDbContext context)
        {
            var elmahContext = new ElmahContext("Elmah.SqlServer"); //MainConnection
            //if(!elmahContext.Database.Exists())
            //{
            //    elmahContext.Database.Create();
            //}
            elmahContext.Database.CreateIfNotExists();

            DirectoryInfo currentDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string solutionDir = currentDir.Parent.Parent.Parent.FullName;

            var scriptPath = Path.Combine(solutionDir, "AcSys.ShiftManager.App/App_Readme/Elmah.SqlServer.sql");
            string script = File.ReadAllText(scriptPath);

            if (SqlHelper.TableDoesNotExist(context.Database.Connection.ConnectionString, "ELMAH_Error"))
            {
                int result = SqlHelper.RunScript(context.Database.Connection.ConnectionString, script);
            }
        }

        int CreateNLogTable(ApplicationDbContext context)
        {
            if (SqlHelper.TableExists(context.Database.Connection.ConnectionString, "Log")) return -1;

            string createNLogTableQuery = @"
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                CREATE TABLE [dbo].[Log] ([Id] [int] IDENTITY(1,1) NOT NULL,[Application] [nvarchar](50) NOT NULL,[Logged] [datetime] NOT NULL,
                    [Level] [nvarchar](50) NOT NULL,[Message] [nvarchar](max) NOT NULL,[UserName] [nvarchar](250) NULL,[ServerName] [nvarchar](max) NULL,
                    [Port] [nvarchar](max) NULL,[Url] [nvarchar](max) NULL,[Https] [bit] NULL,[ServerAddress] [nvarchar](100) NULL,
                    [RemoteAddress] [nvarchar](100) NULL,[Logger] [nvarchar](250) NULL,[Callsite] [nvarchar](max) NULL,[Exception] [nvarchar](max) NULL,
                    CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED ([Id] ASC)
                    WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                ) ON [PRIMARY]";

            int result = context.Database.ExecuteSqlCommand(createNLogTableQuery);
            return result;
        }

        Shift CreateShift(ApplicationDbContext context, User employee, DateTime startTime, DateTime endTime, string title, DateTime? clockinTime = null, DateTime? clockoutTime=null)
        {
            Shift shift = new Shift(startTime, endTime, 30, employee)
            {
                Title = title,

                ClockInTime = clockinTime,
                ClockOutTime = clockoutTime,

                Notes = "{0}\n{1} - {2}\n{3}".FormatWith(title, startTime.ToFormattedTimeString(), endTime.ToFormattedTimeString(), employee == null ? "" : employee.GetFullName())
            };

            if (clockinTime.HasValue)
            {
                shift.ClockIn(clockinTime.Value);

                if (clockoutTime.HasValue)
                {
                    shift.ClockOut(clockoutTime.Value);
                }
            }

            context.Shifts.AddOrUpdate(o => o.Notes, shift);
            context.SaveChanges();

            return shift;
        }

        Message CreateMessage(ApplicationDbContext context, User sender, string subject, string text, params User[] recipients)
        {
            Message message = new Message()
            {
                Sender = sender,
                Subject= subject,
                Text = text,
                SentAt = DateTime.Now
            };

            foreach(User recipient in recipients)
            {
                message.Recipients.Add(recipient);
            }

            context.Messages.AddOrUpdate(o => o.Subject, message);
            context.SaveChanges();

            return message;
        }

        Notification CreateNotification(ApplicationDbContext context, User author, string title, string text, bool isPublic, params Role[] roles)
        {
            Notification notification = new Notification()
            {
                Type = Enums.NotificationType.UserNotification,
                Sender = author,
                Title = title,
                Text = text,
                IsPublic = isPublic,
                SentAt = DateTime.Now
            };

            foreach (Role role in roles)
            {
                IEnumerable<User> roleUsers = role.UserRoles.Select(o => o.User);
                foreach (User user in roleUsers)
                {
                    notification.Recipients.Add(user);
                }
            }

            context.Notifications.AddOrUpdate(o => o.Title, notification);
            context.SaveChanges();

            return notification;
        }

        User CreateUser(ApplicationDbContext context, Role role, string username, string password, string firstName, string lastName, string email
            , string phoneNumber, string mobile, DateTime dateOfBirth)
        {
            User user = new User()
            {
                FirstName = firstName,
                LastName = lastName,

                DateOfBirth = dateOfBirth,

                UserName = username,
                PasswordHash = passwordHasher.HashPassword(password),

                Email = email,
                EmailConfirmed = false,

                //IsActive = true,
                MustChangePassword = false,

                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = false,

                Mobile = mobile,

                TwoFactorEnabled = false,
                LockoutEnabled = false,

                AccessFailedCount = 0,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            UserRole userRole = new UserRole()
            {
                User = user,
                Role = role,

                UserId = user.Id,
                RoleId = role.Id,
            };

            //user.Roles.Add(userRole);
            //user.Roles.Add(role);
            user.UserRoles.Add(userRole);

            //context.Users.Add(user);
            //context.Users.AddOrUpdate(o => o.Name, u => new { u.UserName, u.Email, u.IsActive }, superAdminUser);
            context.Users.AddOrUpdate(o => o.UserName, user);

            //context.UserRoles.AddOrUpdate(o => new { o.UserId, o.RoleId }, userRole);
            context.SaveChanges();

            return user;
        }
    }
}
