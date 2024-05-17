using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using AcSys.ShiftManager.Tests.Data.DbSet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AcSys.ShiftManager.Tests.Data
{
    /// <summary>
    /// Summary description for DataTestsBase
    /// </summary>
    [TestClass]
    public class DataTestsBase
    {
        public DataTestsBase()
        {
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
        public void TestMethod1()
        {
            //
            // TODO: Add test logic here
            //
        }

        protected static Mock<DbSet<T>> CreateMockSet<T>(IQueryable<T> dataForDbSet) where T : class
        {
            //  https://msdn.microsoft.com/en-us/data/dn313107
            //  https://msdn.microsoft.com/en-us/data/dn314429

            var dbsetMock = new Mock<DbSet<T>>();

            dbsetMock.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(dataForDbSet.GetEnumerator()));

            dbsetMock.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(dataForDbSet.Provider));


            //dbsetMock.As<IQueryable<T>>().Setup(m => m.Provider)
            //    .Returns(dataForDbSet.Provider);

            dbsetMock.As<IQueryable<T>>().Setup(m => m.Expression)
                .Returns(dataForDbSet.Expression);
            dbsetMock.As<IQueryable<T>>().Setup(m => m.ElementType)
                .Returns(dataForDbSet.ElementType);
            dbsetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator())
                .Returns(dataForDbSet.GetEnumerator());
            return dbsetMock;
        }
    }
}
