using System;
using AcSys.Core.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcSys.ShiftManager.Tests
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        public void WeekDaysTestMethod()
        {
            DateTime.Today.BeginningOfTheWeek().DayOfWeek.Should().Be(DayOfWeek.Monday);
            DateTime.Today.EndOfTheWeek().DayOfWeek.Should().Be(DayOfWeek.Sunday);

            DayOfWeek firstDayOfWeek = DayOfWeek.Sunday;
            DayOfWeek lastDayOfWeek = DayOfWeek.Saturday;
            DateTime.Today.BeginningOfTheWeek(firstDayOfWeek).DayOfWeek.Should().Be(firstDayOfWeek);
            DateTime.Today.EndOfWeek(firstDayOfWeek).DayOfWeek.Should().Be(lastDayOfWeek);

            foreach (int day in new int[] { 19, 20, 21, 22, 23, 24, 25 })
            {
                DateTime dt = new DateTime(2016, 12, day),
                    beg = dt.BeginningOfTheWeek(),
                    end = dt.EndOfTheWeek();

                beg.DayOfWeek.Should().Be(DayOfWeek.Monday);
                beg.Day.Should().Be(19);

                end.DayOfWeek.Should().Be(DayOfWeek.Sunday);
                end.Day.Should().Be(25);

                dt.BeginningOfTheWeek(firstDayOfWeek).DayOfWeek.Should().Be(firstDayOfWeek);
                dt.EndOfWeek(firstDayOfWeek).DayOfWeek.Should().Be(lastDayOfWeek);
            }
        }

        [TestMethod]
        public void TimeSpanParseTest()
        {
            TimeSpan time = TimeSpan.Parse("11:04:20");
            time.Hours.Should().Be(11);
            time.Minutes.Should().Be(4);
            time.Seconds.Should().Be(20);
        }
    }
}
