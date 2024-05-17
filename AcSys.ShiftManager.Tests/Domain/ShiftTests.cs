using System;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Model.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AcSys.ShiftManager.Service.Shifts;
using AcSys.Core.Email;
using AcSys.ShiftManager.Data.Notifications;
using AcSys.ShiftManager.Data.Messages;
using AcSys.ShiftManager.Data.Users;
using AcSys.ShiftManager.Data.Shifts;
using AcSys.ShiftManager.Data.EF.Identity;
using AcSys.ShiftManager.Data.UnitOfWork;
using Autofac.Extras.NLog;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Data.EF.UnitOfWork;
using AcSys.ShiftManager.Data.EF.Repos.Users;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;
using NLog;
using AcSys.ShiftManager.Data.EF.Repos.ActivityLogs;
using AcSys.ShiftManager.Data.EF.Repos.Shifts;
using AcSys.ShiftManager.Data.EF.Repos.Messages;
using AcSys.ShiftManager.Data.EF.Repos.Notifications;
using System.Threading.Tasks;
using AcSys.Core.Extensions;

namespace AcSys.ShiftManager.Tests.Domain
{
    /// <summary>
    /// Summary description for ShiftTests
    /// </summary>
    [TestClass]
    public class ShiftTests
    {
        public ShiftTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CreateShiftTest()
        {
            DateTime startTime = DateTime.Now; // new DateTime(2017, 01, 01, 9, 0, 0);
            DateTime endTime = startTime.AddHours(-1); //new DateTime(2017, 01, 01, 17, 0, 0);

            Shift shift = null;

            Action act = () => shift = new Shift(startTime, endTime, 30)
            {
                Notes = "notes.......",
            };

            act.ShouldThrow<ApplicationException>()
                .WithMessage("Start time can not be less than 15 minutes in future.");

            startTime = DateTime.Now.AddMinutes(16);
            endTime = startTime.AddHours(-1);

            act.ShouldThrow<ApplicationException>()
                .WithMessage("Shift end time cannot be earlier than it's start time.");

            startTime = DateTime.Now.AddMinutes(16);
            endTime = startTime.AddHours(8);

            act.ShouldNotThrow<ApplicationException>();

            shift.Should().NotBeNull();

            shift.IsOpen.Should().BeTrue();

            shift.Title = shift.ToString();
            shift.ClockIn(DateTime.Now);


            // faking DateTime.Now
            // https://ayende.com/blog/3408/dealing-with-time-in-tests
            // http://stackoverflow.com/questions/43711/whats-a-good-way-to-overwrite-datetime-now-during-testing
        }

        [TestMethod]
        public void ShiftLatenessTests()
        {
            string s = DateTime.Now.ToString("hh:mm tt");

            User emplyee = new User();

            DateTime startTime = new DateTime(2017, 01, 01, 9, 0, 0);
            DateTime endTime = new DateTime(2017, 01, 01, 17, 0, 0);

            Shift shift = new Shift()
            {
                StartTime = startTime,
                EndTime = endTime,

                Employee = emplyee,
                IsOpen = false,

                Notes = "notes.......",
                Status = Model.Helpers.Enums.ShiftStatus.Due
            };

            shift.ClockIn(startTime.AddMinutes(1));

            shift.Status.Should().Be(Enums.ShiftStatus.Due);
            shift.StartStatus.Should().Be(Enums.ShiftStartStatus.Late);
            shift.EndStatus.Should().Be(Enums.ShiftEndStatus.None);
            shift.LateMins.Should().Be(1);

            shift.ClockIn(startTime.AddMinutes(-1));

            shift.Status.Should().Be(Enums.ShiftStatus.Due);
            shift.StartStatus.Should().Be(Enums.ShiftStartStatus.OnTime);
            shift.EndStatus.Should().Be(Enums.ShiftEndStatus.None);


            shift.ClockOut(endTime.AddMinutes(-1));

            shift.Status.Should().Be(Enums.ShiftStatus.Complete);
            shift.EndStatus.Should().Be(Enums.ShiftEndStatus.Early);
            shift.ClockedMins.Should().Be(479);

            shift.ClockOut(endTime.AddMinutes(1));

            shift.Status.Should().Be(Enums.ShiftStatus.Complete);
            shift.EndStatus.Should().Be(Enums.ShiftEndStatus.OnTime);
            shift.ClockedMins.Should().Be(481);
        }

        [TestMethod]
        public void MyTestMethod()
        {
            double attendancePercentage = (double)14 / 44;
            attendancePercentage.Should().Be(33);

            double lateAttendancePercentage = 4/ 44 * 100;
            lateAttendancePercentage.Should().Be(3);
        }

        [TestMethod]
        public async Task ShiftUtcDateTimeTests()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            context.Database.CreateIfNotExists();

            //IUnitOfWork unitOfWork = new UnitOfWork(context);
            //IEmailService emailService = new EmailService();
            //IIdentityMessageService identityEmailService = new IdentityEmailService(emailService);
            IRoleRepository roleRepo = new RoleRepository(context);
            IUserRepository userRepo = new UserRepository(context);

            //ApplicationRoleManager roleManager = new ApplicationRoleManager(roleRepo as IRoleStore<Role, Guid>);

            //IDataProtectionProvider dataProtectionProvider = new DpapiDataProtectionProvider("AcSys.ShiftManager");
            //IUserTokenProvider<User, Guid> userTokenProvider = new DataProtectorTokenProvider<User, Guid>(dataProtectionProvider.Create("EmailConfirmation"));
            //ApplicationUserManager userManager = new ApplicationUserManager(userRepo as IUserStore<User, Guid>, identityEmailService, dataProtectionProvider, userTokenProvider);
            IShiftRepository repo = new ShiftRepository(context);

            //IEmployeeGroupRepository empGroupRepo = new EmployeeGroupRepository(context);
            //IMessageRepository messageRepo = new MessageRepository(context);
            //INotificationRepository notificationRepo = new NotificationRepository(context);
            //Autofac.Extras.NLog.ILogger logger = new LoggerAdapter(LogManager.GetCurrentClassLogger());

            //IShiftsService service = new ShiftsService(unitOfWork, roleManager, userManager, repo, userRepo, empGroupRepo, messageRepo, notificationRepo, logger, emailService);

            Role employeeRole = new Role() { Name = AppConstants.RoleNames.Employee };
            await roleRepo.AddAsync(employeeRole);

            User employeeUser = new User() { Email = "mymg55@yahoo.com", FirstName = "Mohamed", LastName = "Ghassar", UserName = "ghassar", Mobile = "2343252", DateOfBirth = DateTime.Now.AddDays(9999) };
            await userRepo.AddAsync(employeeUser);

            DateTime startTime = new DateTime(2017, 01, 01, 9, 0, 0);
            DateTime endTime = new DateTime(2017, 01, 01, 17, 0, 0);

            startTime = startTime.ToUniversalTime();
            endTime = endTime.ToUniversalTime();

            Shift shift = new Shift()
            {
                StartTime = startTime,
                EndTime = endTime,

                Employee = employeeUser,
                IsOpen = false,

                Title = "{0} - {1}".FormatWith(startTime.ToShortTimeString(), endTime.ToShortTimeString()),
                TotalBreakMins = 30,

                Notes = "notes.......",
                Status = Enums.ShiftStatus.Due
            };
            await repo.AddAsync(shift);

            shift.Id.Should().NotBeEmpty();

            shift.StartTime.Should().Be(startTime);

            DateTime dt = shift.StartTime;
            DateTime dtu = shift.StartTime.ToUniversalTime();
            dtu.Should().Be(dtu);

            shift.StartTime.ToUniversalTime().Should().Be(startTime);

            shift.ClockIn(startTime.AddMinutes(1));

            shift.Status.Should().Be(Enums.ShiftStatus.Due);
            shift.StartStatus.Should().Be(Enums.ShiftStartStatus.Late);
            shift.EndStatus.Should().Be(Enums.ShiftEndStatus.None);
            shift.LateMins.Should().Be(1);

            shift.ClockIn(startTime.AddMinutes(-1));

            shift.Status.Should().Be(Enums.ShiftStatus.Due);
            shift.StartStatus.Should().Be(Enums.ShiftStartStatus.OnTime);
            shift.EndStatus.Should().Be(Enums.ShiftEndStatus.None);


            shift.ClockOut(endTime.AddMinutes(-1));

            shift.Status.Should().Be(Enums.ShiftStatus.Complete);
            shift.EndStatus.Should().Be(Enums.ShiftEndStatus.Early);
            shift.ClockedMins.Should().Be(479);

            shift.ClockOut(endTime.AddMinutes(1));

            shift.Status.Should().Be(Enums.ShiftStatus.Complete);
            shift.EndStatus.Should().Be(Enums.ShiftEndStatus.OnTime);
            shift.ClockedMins.Should().Be(481);
        }
    }
}
