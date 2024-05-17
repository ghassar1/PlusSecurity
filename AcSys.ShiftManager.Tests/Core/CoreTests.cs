using System;
using System.Linq;
using AcSys.Core.AspNet;
using AcSys.Core.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcSys.ShiftManager.Tests.Core
{
    /// <summary>
    /// Summary description for CoreTests
    /// </summary>
    [TestClass]
    public class CoreTests
    {
        public CoreTests()
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
        public void MachineKeyGeneratorTest()
        {
            MachineKey machineKey = MachineKeyGenerator.Create();
            machineKey.Should().NotBeNull();
            machineKey.Tag.Should().NotBeNullOrWhiteSpace();
            machineKey.DecryptionKey.Should().NotBeNullOrWhiteSpace();
            machineKey.ValidationKey.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        public void IntegerToGuidTest()
        {
            Random rand = new Random();
            int value = rand.Next();
            Guid id = value.ToGuid();
            Console.WriteLine("{0} to Guid: {1}".FormatWith(value.ToString().PadLeft(10, '0'), id));
            id.Should().IsNotNull();
            id.Should().NotBeEmpty();

            for (int i = 1; i > 0 && i <= int.MaxValue; i += i)
            {
                //value = rand.Next();
                value = i;
                id = value.ToGuid();
                Console.WriteLine("{0} to Guid: {1}".FormatWith(value.ToString().PadLeft(10, '0'), id));
                id.Should().IsNotNull();
                id.Should().NotBeEmpty();
            }
        }

        [TestMethod]
        public void MyTestMethod()
        {
            string invalid = "$am$it$";
            string s = invalid.Trim(new char[] { '$' });
            Console.WriteLine(s);

            DateTime now = DateTime.Now;
            DateTime later = now.AddMinutes(-10);
            long diff = now.DateDiff("minute", later);
            diff.Should().BeGreaterOrEqualTo(-10);

            later = now.AddHours(8);

            diff = now.DateDiff("hour", later);
            diff.Should().BeGreaterOrEqualTo(8);

            diff = later.DateDiff("minute", now);
            diff.Should().BeGreaterOrEqualTo(-480);

            DateTime dt = DateTime.Now;
            DateTime dtu = dt.ToUniversalTime();
            DateTime dtuu = dtu.ToUniversalTime();
        }
    }
}
