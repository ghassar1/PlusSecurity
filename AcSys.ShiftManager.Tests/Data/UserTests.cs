using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcSys.Core.Data.Querying;
using AcSys.Core.Data.Specifications;
using AcSys.Core.Extensions;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Data.EF.Repos.Users;
using AcSys.ShiftManager.Data.Users;
using AcSys.ShiftManager.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AcSys.ShiftManager.Tests.Data
{
    /// <summary>
    /// Summary description for SpecificationsTests
    /// </summary>
    [TestClass]
    public class UserTests : DataTestsBase
    {
        public UserTests()
        {
        }

        protected static IQueryable<User> Users { get; set; }
        
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Users = Enumerable.Range(1, 100)
                .Select(x => new User()
                {
                    FirstName = "FN{0:D2}".FormatWith(x),
                    LastName = "LN{0:D2}".FormatWith(x),
                    Email = "email{0:D2}@email.com".FormatWith(x),
                    UserName = "email{0:D2}@email.com".FormatWith(x),
                    DateOfBirth = DateTime.Now.AddYears(-30).AddDays(-1 * x),
                    Mobile = "{0}{1}".FormatWith("44755", new Random().Next(6780000, 6780000).ToString()),
                    PhoneNumber = "{0}{1}".FormatWith("44755", new Random().Next(6780000, 6780000).ToString())
                }).AsQueryable();
        }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup() { }
        
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() { }
        
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() { }

        [TestMethod]
        public void FindUsersQuery_Satisfies()
        {
            User user = new User() { FirstName = "Mohamed", LastName = "Ghassar", UserName = "mymg55@yahoo.com", Email = "mymg55@yahoo.com" };
            var query = new FindUsersQuery() { SearchCriteria = "mymg55@yahoo.com" };
            var spec = query.ToSpec();
            var satisfied = spec.IsSatisfiedBy(user);
            satisfied.Should().BeTrue();
        }

        [TestMethod]
        public async Task ActiveUsersByUsernameSpec_FetshesData()
        {
            var dbsetMock = CreateMockSet<User>(Users);

            Mock<ApplicationDbContext> contextMock = new Mock<ApplicationDbContext>();
            contextMock.Setup(c => c.Set<User>()).Returns(dbsetMock.Object);
            
            IUserRepository repo = new UserRepository(contextMock.Object);
            
            var spec = new ActiveUsersByUsernameSpec("mymg55@yahoo.com");
            var result = await repo.Find(spec);

            result.Should().NotBeNull();
            result.TotalRecords.Should().BePositive();
            result.FilteredRecords.Should().BePositive();
            result.Records.Should().NotBeNullOrEmpty();
            result.Records.Should().NotContainNulls();
            result.Records.Should().OnlyHaveUniqueItems();
            result.Records.Should().HaveCount(1);
        }

        [TestMethod]
        public async Task GetUserBySpecificationsUsingAndOrOrderBySkip()
        {
            var dbsetMock = CreateMockSet<User>(Users);

            Mock<ApplicationDbContext> contextMock = new Mock<ApplicationDbContext>();
            contextMock.Setup(c => c.Set<User>()).Returns(dbsetMock.Object);

            IUserRepository repo = new UserRepository(contextMock.Object);

            var spec = new Specification<User>(o => o.UserName.Contains("0"))
                .Or(o=>o.Email.Contains("5"))
                .OrderBy(o => o.DateOfBirth)
                .Skip(2)
                .Take(2);

            var result = await repo.Find(spec);

            result.Should().NotBeNull();
            result.TotalRecords.Should().BePositive();
            result.FilteredRecords.Should().BePositive();
            result.Records.Should().NotBeNullOrEmpty();
            result.Records.Should().NotContainNulls();
            result.Records.Should().OnlyHaveUniqueItems();
            result.Records.Should().HaveCount(2);
        }
        
        [TestMethod]
        public async Task FindUserQueryTest()
        {
            var dbsetMock = CreateMockSet<User>(Users);

            Mock<ApplicationDbContext> contextMock = new Mock<ApplicationDbContext>();
            contextMock.Setup(c => c.Set<User>()).Returns(dbsetMock.Object);

            IUserRepository repo = new UserRepository(contextMock.Object);

            FindUsersQuery query = new FindUsersQuery() { SearchCriteria = "email1" };

            ISearchResult<User> result = await repo.Find(query);

            result.Should().NotBeNull();

            result.Query.PageNo.Should().Be(query.PageNo);
            result.Query.PageSize.Should().Be(query.PageSize);

            result.TotalRecords.Should().BePositive();
            result.TotalRecords.Should().Be(11);

            result.FilteredRecords.Should().BePositive();
            result.FilteredRecords.Should().Be(10);

            result.Records.Should().NotBeNullOrEmpty();
            result.Records.Should().NotContainNulls();
            result.Records.Should().OnlyHaveUniqueItems();
            result.Records.Should().HaveCount(10);
            

            query = new FindUsersQuery() { SearchCriteria = "email1", PageNo = 2 };

            result = await repo.Find(query);

            result.Should().NotBeNull();

            result.Query.PageNo.Should().Be(query.PageNo);
            result.Query.PageSize.Should().Be(query.PageSize);

            result.TotalRecords.Should().BePositive();
            result.TotalRecords.Should().Be(11);

            result.FilteredRecords.Should().BePositive();
            result.FilteredRecords.Should().Be(1);

            result.Records.Should().NotBeNullOrEmpty();
            result.Records.Should().NotContainNulls();
            result.Records.Should().OnlyHaveUniqueItems();
            result.Records.Should().HaveCount(1);
        }

        [TestMethod]
        public async Task FindUserNotificationsQueryTest()
        {
            var roles = Enumerable.Range(1, 2)
                .Select(i => new Role()
                {
                    Id = i.ToGuid(),
                    Name = "Role_{0:D2}".FormatWith(i)
                }).ToList();
            var role1 = roles.FirstOrDefault();
            var role2 = roles.LastOrDefault();

            var users = Enumerable.Range(10, 11)
                .Select(i => new User()
                {
                    Id = i.ToGuid(),
                    FirstName = "FN{0:D2}".FormatWith(i),
                    LastName = "LN{0:D2}".FormatWith(i),
                    Email = "email{0:D2}@email.com".FormatWith(i),
                    UserName = "email{0:D2}".FormatWith(i),
                    DateOfBirth = DateTime.Now.AddYears(-30).AddDays(-1 * i),
                    Mobile = "{0}{1}".FormatWith("44755", new Random().Next(6780000, 6780000).ToString()),
                    PhoneNumber = "{0}{1}".FormatWith("44755", new Random().Next(6780000, 6780000).ToString())
                }).ToList();

            var user = users.FirstOrDefault();
            var user2 = users.LastOrDefault();

            int x = 101;
            var notif = new Notification()
            {
                Id = x.ToGuid(),
                Title = "Title_{0:D2}".FormatWith(x),
                Text = "LN{0:D2}".FormatWith(x)
            };
            //notif.Recipients.Add(role1);
            role1.UserRoles.Select(o => o.User).ToList().ForEach((u) => { notif.Recipients.Add(u); });
            
            user.AuthoredNotifications.Add(notif);

            var dbsetMock = CreateMockSet<User>(Users);

            Mock<ApplicationDbContext> contextMock = new Mock<ApplicationDbContext>();
            contextMock.Setup(c => c.Set<User>()).Returns(dbsetMock.Object);

            IUserRepository repo = new UserRepository(contextMock.Object);

            FindUsersQuery query = new FindUsersQuery() { SearchCriteria = "email1" };

            ISearchResult<User> result = await repo.Find(query);

            result.Should().NotBeNull();

            result.Query.PageNo.Should().Be(query.PageNo);
            result.Query.PageSize.Should().Be(query.PageSize);

            result.TotalRecords.Should().BePositive();
            result.TotalRecords.Should().Be(11);

            result.FilteredRecords.Should().BePositive();
            result.FilteredRecords.Should().Be(10);

            result.Records.Should().NotBeNullOrEmpty();
            result.Records.Should().NotContainNulls();
            result.Records.Should().OnlyHaveUniqueItems();
            result.Records.Should().HaveCount(10);

            query = new FindUsersQuery() { SearchCriteria = "email1", PageNo = 2 };

            result = await repo.Find(query);

            result.Should().NotBeNull();

            result.Query.PageNo.Should().Be(query.PageNo);
            result.Query.PageSize.Should().Be(query.PageSize);

            result.TotalRecords.Should().BePositive();
            result.TotalRecords.Should().Be(11);

            result.FilteredRecords.Should().BePositive();
            result.FilteredRecords.Should().Be(1);

            result.Records.Should().NotBeNullOrEmpty();
            result.Records.Should().NotContainNulls();
            result.Records.Should().OnlyHaveUniqueItems();
            result.Records.Should().HaveCount(1);
        }
    }
}
