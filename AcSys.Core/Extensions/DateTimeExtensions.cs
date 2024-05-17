using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace AcSys.Core.Extensions
{
    public static class DateTimeExtensions
    {
        #region Formatting

        public static string DefaultDateFormat = "dd MMM, yyyy";
        public static string DefaultTimeFormat = "dd MMM, yyyy hh:mm:ss tt";
        public static string DefaultShortTimeFormat = "hh:mm:ss tt";

        public static string ToFormattedDateString(this DateTime input)
        {
            string date = input.ToString(DefaultDateFormat);
            return date;
        }

        public static string ToFormattedDateString(this DateTime? input)
        {
            string date = input.HasValue ? input.Value.ToString(DefaultDateFormat) : "";
            return date;
        }

        public static string ToFormattedTimeString(this DateTime input)
        {
            string date = input.ToString(DefaultTimeFormat);
            return date;
        }

        public static string ToFormattedTimeString(this DateTime? input)
        {
            string date = input.HasValue ? input.Value.ToString(DefaultTimeFormat) : "";
            return date;
        }

        public static string ToShortFormattedTimeString(this DateTime input)
        {
            string date = input.ToString(DefaultShortTimeFormat);
            return date;
        }

        public static string ToShortFormattedTimeString(this DateTime? input)
        {
            string date = input.HasValue ? input.Value.ToString(DefaultShortTimeFormat) : "";
            return date;
        }

        #endregion

        #region Common

        #endregion

        private static readonly DateTime MinDate = new DateTime(1900, 1, 1);
        private static readonly DateTime MaxDate = new DateTime(2100, 12, 31, 23, 59, 59, 999);

        public static bool IsValid(this DateTime target)
        {
            return (target >= MinDate) && (target <= MaxDate);
        }

        public static bool IsNullOrEmpty(this DateTime? value)
        {
            return value == null || !value.HasValue;
        }

        public static bool IsNotNullOrEmpty(this DateTime? value)
        {
            return !value.IsNullOrEmpty();
        }

        public static bool IsInFuture(this DateTime value)
        {
            return DateTime.Compare(value, DateTime.Now) > 0;
        }

        public static bool IsAfter(this DateTime value, DateTime targetDate)
        {
            return value > targetDate;
        }

        public static bool IsBefore(this DateTime value, DateTime targetDate)
        {
            return value < targetDate;
        }

        #region Financial Year

        public static DateTime FinancialYearStart(this DateTime date, int yearEndMonth, int yearEndDay)
        {
            DateTime yearEnd = date.FinancialYearEnd(yearEndMonth, yearEndDay);
            DateTime yearStart = yearEnd.AddDays(1).AddYears(-1).BeginningOfTheDay();
            yearStart = yearStart.Date;
            return yearStart;
        }

        public static DateTime FinancialYearEnd(this DateTime date, int yearEndMonth, int yearEndDay)
        {
            DateTime yearEnd = new DateTime(date.Month > yearEndMonth ? date.Year + 1 : date.Year, yearEndMonth, yearEndDay);
            yearEnd = yearEnd.Date;
            return yearEnd;
        }

        public static DateTime LastFinancialYearStart(this DateTime dateTime, int yearEndMonth, int yearEndDay)
        {
            //DateTime newDate = new DateTime(currentFinYearStart.Year - 1, 4, 1);
            DateTime currentFinYearStart = dateTime.FinancialYearStart(yearEndMonth, yearEndDay);
            DateTime newDate = currentFinYearStart.AddYears(-1);
            return newDate;
        }

        public static DateTime LastFinancialYearEnd(this DateTime dateTime, int yearEndMonth, int yearEndDay)
        {
            //DateTime newDate = new DateTime(currentFinYearEnd.Year - 1, 3, 31);

            DateTime currentFinYearEnd = dateTime.FinancialYearEnd(yearEndMonth, yearEndDay);
            DateTime newDate = currentFinYearEnd.AddYears(-1);
            return newDate;
        }

        public static DateTime GetYearStart(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1);
        }

        public static DateTime GetYearEnd(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 12, 31);
        }

        //public static DateTime GetFirstDayOfMonth(this DateTime dateTime)
        //{
        //    return new DateTime(dateTime.Year, dateTime.Month, 1);
        //}

        //public static DateTime GetLastDayOfMonth(this DateTime dateTime)
        //{
        //    DateTime firstDayOfTheMonth = dateTime.GetFirstDayOfMonth();
        //    return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        //}

        #endregion



        #region ExtensionMethod.net

        /*********************************************************************
        *************http://extensionmethod.net/csharp/datetime***************
        *********************************************************************/

        public static bool IsLastDayOfTheMonth(this DateTime dateTime)
        {
            //return dateTime == new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1);
            return dateTime.Date == dateTime.EndOfTheMonth().Date;
        }

        public static bool IsFirstDayOfTheMonth(this DateTime dateTime)
        {
            return dateTime.Date == dateTime.BeginningOfTheMonth().Date;
        }

        public static bool IsDayOfTheSameMonth(this DateTime dateTime, DateTime target)
        {
            return dateTime.Month == target.Month && dateTime.Year == target.Year;
        }

        public static Boolean IsLeapDay(this DateTime date)
        {
            return (date.Month == 2 && date.Day == 29);
        }

        /// <summary>
        /// Returns a formatted date or emtpy string
        /// </summary>
        /// <param name="t">DateTime instance or null</param>
        /// <param name="format">datetime formatstring </param>
        /// <returns></returns>
        public static string ToString(this DateTime? t, string format)
        {
            if (t != null)
            {
                return t.Value.ToString(format);
            }

            return "";
        }

        public static DateTime ThisWeekMonday(this DateTime dt)
        {
            var today = DateTime.Now;
            return new GregorianCalendar().AddDays(today, -((int)today.DayOfWeek) + 1);
        }

        public static DateTime BeginningOfTheMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime EndOfTheMonth(this DateTime date)
        {
            var endOfTheMonth = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
            return endOfTheMonth;
        }

        public static DateTime EndOfTheDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        public static DateTime BeginningOfTheDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static bool IsWeekend(this DateTime value)
        {
            return (value.DayOfWeek == DayOfWeek.Sunday || value.DayOfWeek == DayOfWeek.Saturday);
        }

        public static bool IsWeekday(this DateTime value)
        {
            return !value.IsWeekend();
        }

        public static bool IsWeekend(this DayOfWeek d)
        {
            return !d.IsWeekday();
        }

        public static bool IsWeekday(this DayOfWeek d)
        {
            switch (d)
            {
                case DayOfWeek.Sunday:
                case DayOfWeek.Saturday: return false;

                default: return true;
            }
        }

        public static DateTime BeginningOfTheWeek(this DateTime dt)
        {
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            DayOfWeek firstDayOfWeek = ci.DateTimeFormat.FirstDayOfWeek;
            return dt.BeginningOfTheWeek(firstDayOfWeek);
        }

        public static DateTime BeginningOfTheWeek(this DateTime dt, DayOfWeek firstDayOfWeek)
        {
            DateTime dateTime = dt.Date;
            int diff = dateTime.DayOfWeek - firstDayOfWeek;
            if (diff < 0) diff += 7;
            return dateTime.AddDays(-1 * diff).Date;
        }

        public static DateTime BeginningOfLastWeek(this DateTime dateTime)
        {
            DateTime newDate = dateTime.BeginningOfTheWeek();
            newDate = newDate.AddDays(-1);
            newDate = newDate.BeginningOfTheWeek();
            newDate = newDate.BeginningOfTheDay();
            return newDate;
        }

        public static DateTime EndOfLastWeek(this DateTime dateTime)
        {
            DateTime newDate = dateTime.BeginningOfTheWeek();
            newDate = newDate.AddDays(-1);
            newDate = newDate.EndOfTheWeek();
            newDate = newDate.EndOfTheDay();
            return newDate;
        }

        public static DateTime EndOfTheWeek(this DateTime dt)
        {
            return dt.BeginningOfTheWeek().AddDays(6);
        }

        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek firstDayOfWeek)
        {
            return dt.BeginningOfTheWeek(firstDayOfWeek).AddDays(6);
        }

        public static DateTime AddWorkdays(this DateTime d, int days)
        {
            // start from a weekday
            while (d.DayOfWeek.IsWeekday()) d = d.AddDays(1.0);
            for (int i = 0; i < days; ++i)
            {
                d = d.AddDays(1.0);
                while (d.DayOfWeek.IsWeekday()) d = d.AddDays(1.0);
            }
            return d;
        }

        static public int Age(this DateTime dateOfBirth)
        {
            if (DateTime.Today.Month < dateOfBirth.Month ||
            DateTime.Today.Month == dateOfBirth.Month &&
             DateTime.Today.Day < dateOfBirth.Day)
            {
                return DateTime.Today.Year - dateOfBirth.Year - 1;
            }
            else
                return DateTime.Today.Year - dateOfBirth.Year;
        }

        //public static string ToStringFormat(this DateTime source, Expression<Func<DateTimeFormat>> dateTimeFormat)
        //{
        //    var dateTimeFormatCompiled = dateTimeFormat.Compile().Invoke();

        //    var dateTimeStringFormat = StringEnum.GetStringValue(dateTimeFormatCompiled);

        //    var currentCulture = Thread.CurrentThread.CurrentCulture;

        //    return source.ToString(dateTimeStringFormat, currentCulture);
        //}

        public static IEnumerable<DateTime> GetDateRangeTo(this DateTime self, DateTime toDate)
        {
            int days = new TimeSpan(toDate.Ticks - self.Ticks).Days;
            var range = Enumerable.Range(0, days);

            return from p in range
                   select self.Date.AddDays(p);
        }

        public static IEnumerable<DateTime> GetDateRangeInclusiveTo(this DateTime self, DateTime toDate)
        {
            int days = new TimeSpan(toDate.Ticks - self.Ticks).Days + 1;
            var range = Enumerable.Range(0, days);

            return from p in range
                   select self.Date.AddDays(p);
        }

        public static DateTime LastDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// DateDiff in SQL style. 
        /// Datepart implemented: 
        ///     "year" (abbr. "yy", "yyyy"), 
        ///     "quarter" (abbr. "qq", "q"), 
        ///     "month" (abbr. "mm", "m"), 
        ///     "day" (abbr. "dd", "d"), 
        ///     "week" (abbr. "wk", "ww"), 
        ///     "hour" (abbr. "hh"), 
        ///     "minute" (abbr. "mi", "n"), 
        ///     "second" (abbr. "ss", "s"), 
        ///     "millisecond" (abbr. "ms").
        /// </summary>
        /// <param name="DatePart"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static Int64 DateDiff(this DateTime StartDate, String DatePart, DateTime EndDate)
        {
            Int64 DateDiffVal = 0;
            Calendar cal = Thread.CurrentThread.CurrentCulture.Calendar;
            TimeSpan ts = new TimeSpan(EndDate.Ticks - StartDate.Ticks);
            switch (DatePart.ToLower().Trim())
            {
                #region year
                case "year":
                case "yy":
                case "yyyy":
                    DateDiffVal = (Int64)(cal.GetYear(EndDate) - cal.GetYear(StartDate));
                    break;
                #endregion

                #region quarter
                case "quarter":
                case "qq":
                case "q":
                    DateDiffVal = (Int64)((((cal.GetYear(EndDate)
                                        - cal.GetYear(StartDate)) * 4)
                                        + ((cal.GetMonth(EndDate) - 1) / 3))
                                        - ((cal.GetMonth(StartDate) - 1) / 3));
                    break;
                #endregion

                #region month
                case "month":
                case "mm":
                case "m":
                    DateDiffVal = (Int64)(((cal.GetYear(EndDate)
                                        - cal.GetYear(StartDate)) * 12
                                        + cal.GetMonth(EndDate))
                                        - cal.GetMonth(StartDate));
                    break;
                #endregion

                #region day
                case "day":
                case "d":
                case "dd":
                    DateDiffVal = (Int64)ts.TotalDays;
                    break;
                #endregion

                #region week
                case "week":
                case "wk":
                case "ww":
                    DateDiffVal = (Int64)(ts.TotalDays / 7);
                    break;
                #endregion

                #region hour
                case "hour":
                case "hh":
                    DateDiffVal = (Int64)ts.TotalHours;
                    break;
                #endregion

                #region minute
                case "minute":
                case "mi":
                case "n":
                    DateDiffVal = (Int64)ts.TotalMinutes;
                    break;
                #endregion

                #region second
                case "second":
                case "ss":
                case "s":
                    DateDiffVal = (Int64)ts.TotalSeconds;
                    break;
                #endregion

                #region millisecond
                case "millisecond":
                case "ms":
                    DateDiffVal = (Int64)ts.TotalMilliseconds;
                    break;
                #endregion

                default:
                    throw new Exception(String.Format("DatePart \"{0}\" is unknown", DatePart));
            }
            return DateDiffVal;
        }

        public static Boolean IsBetween(this DateTime dt, DateTime startDate, DateTime endDate, Boolean compareTime = false)
        {
            return compareTime ?
               dt >= startDate && dt <= endDate :
               dt.Date >= startDate.Date && dt.Date <= endDate.Date;
        }

        /// <summary>
        /// Converts a System.DateTime object to Unix timestamp
        /// </summary>
        /// <returns>The Unix timestamp</returns>
        public static long ToUnixTimestamp(this DateTime date)
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan unixTimeSpan = date - unixEpoch;

            return (long)unixTimeSpan.TotalSeconds;
        }

        public static DateTime? ToDateTime(this string s)
        {
            DateTime dtr;
            var tryDtr = DateTime.TryParse(s, out dtr);
            return (tryDtr) ? dtr : new DateTime?();
        }

        public static bool IsInRange(this DateTime currentDate, DateTime beginDate, DateTime endDate)
        {
            return (currentDate >= beginDate && currentDate <= endDate);
        }

        /// <summary>
        /// Convert DateTime to Shamsi Date (YYYY/MM/DD)
        /// </summary>
        public static string ToShamsiDateYMD(this DateTime date)
        {
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            int intYear = PC.GetYear(date);
            int intMonth = PC.GetMonth(date);
            int intDay = PC.GetDayOfMonth(date);
            return (intYear.ToString() + "/" + intMonth.ToString() + "/" + intDay.ToString());
        }
        /// <summary>
        /// Convert DateTime to Shamsi Date (DD/MM/YYYY)
        /// </summary>
        public static string ToShamsiDateDMY(this DateTime date)
        {
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            int intYear = PC.GetYear(date);
            int intMonth = PC.GetMonth(date);
            int intDay = PC.GetDayOfMonth(date);
            return (intDay.ToString() + "/" + intMonth.ToString() + "/" + intYear.ToString());
        }
        /// <summary>
        /// Convert DateTime to Shamsi String 
        /// </summary>
        public static string ToShamsiDateString(this DateTime date)
        {
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            int intYear = PC.GetYear(date);
            int intMonth = PC.GetMonth(date);
            int intDayOfMonth = PC.GetDayOfMonth(date);
            DayOfWeek enDayOfWeek = PC.GetDayOfWeek(date);
            string strMonthName, strDayName;
            switch (intMonth)
            {
                case 1:
                    strMonthName = "فروردین";
                    break;
                case 2:
                    strMonthName = "اردیبهشت";
                    break;
                case 3:
                    strMonthName = "خرداد";
                    break;
                case 4:
                    strMonthName = "تیر";
                    break;
                case 5:
                    strMonthName = "مرداد";
                    break;
                case 6:
                    strMonthName = "شهریور";
                    break;
                case 7:
                    strMonthName = "مهر";
                    break;
                case 8:
                    strMonthName = "آبان";
                    break;
                case 9:
                    strMonthName = "آذر";
                    break;
                case 10:
                    strMonthName = "دی";
                    break;
                case 11:
                    strMonthName = "بهمن";
                    break;
                case 12:
                    strMonthName = "اسفند";
                    break;
                default:
                    strMonthName = "";
                    break;
            }

            switch (enDayOfWeek)
            {
                case DayOfWeek.Friday:
                    strDayName = "جمعه";
                    break;
                case DayOfWeek.Monday:
                    strDayName = "دوشنبه";
                    break;
                case DayOfWeek.Saturday:
                    strDayName = "شنبه";
                    break;
                case DayOfWeek.Sunday:
                    strDayName = "یکشنبه";
                    break;
                case DayOfWeek.Thursday:
                    strDayName = "پنجشنبه";
                    break;
                case DayOfWeek.Tuesday:
                    strDayName = "سه شنبه";
                    break;
                case DayOfWeek.Wednesday:
                    strDayName = "چهارشنبه";
                    break;
                default:
                    strDayName = "";
                    break;
            }

            return (string.Format("{0} {1} {2} {3}", strDayName, intDayOfMonth, strMonthName, intYear));
        }

        public static bool IsLeapYear(this DateTime value)
        {
            return (System.DateTime.DaysInMonth(value.Year, 2) == 29);
        }

        public static string ToFriendlyDateString(this DateTime Date)
        {
            string FormattedDate = "";
            if (Date.Date == DateTime.Today)
            {
                FormattedDate = "Today";
            }
            else if (Date.Date == DateTime.Today.AddDays(-1))
            {
                FormattedDate = "Yesterday";
            }
            else if (Date.Date > DateTime.Today.AddDays(-6))
            {
                // *** Show the Day of the week
                FormattedDate = Date.ToString("dddd").ToString();
            }
            else
            {
                FormattedDate = Date.ToString("MMMM dd, yyyy");
            }

            //append the time portion to the output
            FormattedDate += " @ " + Date.ToString("t").ToLower();
            return FormattedDate;
        }

        /// <summary>
        /// Converts a regular DateTime to a RFC822 date string.
        /// </summary>
        /// <returns>The specified date formatted as a RFC822 date string.</returns>
        public static string ToRFC822DateString(this DateTime date)
        {
            int offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
            string timeZone = "+" + offset.ToString().PadLeft(2, '0');
            if (offset < 0)
            {
                int i = offset * -1;
                timeZone = "-" + i.ToString().PadLeft(2, '0');
            }
            return date.ToString("ddd, dd MMM yyyy HH:mm:ss " + timeZone.PadRight(5, '0'), System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        }

        /// <summary>
        /// Get the elapsed time since the input DateTime
        /// </summary>
        /// <param name="input">Input DateTime</param>
        /// <returns>Returns a TimeSpan value with the elapsed time since the input DateTime</returns>
        /// <example>
        /// TimeSpan elapsed = dtStart.Elapsed();
        /// </example>
        /// <seealso cref="ElapsedSeconds()"/>
        public static TimeSpan Elapsed(this DateTime input)
        {
            return DateTime.Now.Subtract(input);
        }

        public static TimeSpan TimeElapsed(this DateTime date)
        {
            return DateTime.Now - date;
        }

        public static string NullDateToString(this DateTime? dt, string format = "M/d/yyyy", string nullResult = "")
        {
            if (dt.HasValue)
                return dt.Value.ToString(format);
            else
                return nullResult;
        }

        /// <summary>
        /// Gest the elapsed seconds since the input DateTime
        /// </summary>
        /// <param name="input">Input DateTime</param>
        /// <returns>Returns a Double value with the elapsed seconds since the input DateTime</returns>
        /// <example>
        /// Double elapsed = dtStart.ElapsedSeconds();
        /// </example>
        /// <seealso cref="Elapsed()"/>
        public static Double ElapsedSeconds(this DateTime input)
        {
            return DateTime.Now.Subtract(input).TotalSeconds;
        }

        public static string ToOracleSqlDate(this DateTime dateTime)
        {
            return String.Format("to_date('{0}','dd.mm.yyyy hh24.mi.ss')", dateTime.ToString("dd.MM.yyyy HH:mm:ss"));
        }

        public static string LengthOfTime(this DateTime date)
        {
            TimeSpan lengthOfTime = DateTime.Now.Subtract(date);
            if (lengthOfTime.Minutes == 0)
                return lengthOfTime.Seconds.ToString() + "s";
            else if (lengthOfTime.Hours == 0)
                return lengthOfTime.Minutes.ToString() + "m";
            else if (lengthOfTime.Days == 0)
                return lengthOfTime.Hours.ToString() + "h";
            else
                return lengthOfTime.Days.ToString() + "d";
        }

        public static DateTime ThisWeekFriday(this DateTime dt)
        {
            var today = DateTime.Now;
            return new GregorianCalendar().AddDays(today, -((int)today.DayOfWeek) + 5);
        }

        public static DateTime NextSunday(this DateTime dt)
        {
            return new GregorianCalendar().AddDays(dt, -((int)dt.DayOfWeek) + 7);
        }

        public static DateTime AddTime(this DateTime date, int hour, int minutes)
        {
            return date + new TimeSpan(hour, minutes, 0);
        }

        /// <summary>
        /// Compute dateTime difference precisely
        /// Alex-LEWIS, 2015-08-11
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static double GetTotalMonthDiff(this DateTime dt1, DateTime dt2)
        {
            var l = dt1 < dt2 ? dt1 : dt2;
            var r = dt1 >= dt2 ? dt1 : dt2;
            var lDfM = DateTime.DaysInMonth(l.Year, l.Month);
            var rDfM = DateTime.DaysInMonth(r.Year, r.Month);

            var dayFixOne = l.Day == r.Day
              ? 0d
              : l.Day > r.Day
                ? r.Day * 1d / rDfM - l.Day * 1d / lDfM
                : (r.Day - l.Day) * 1d / rDfM;

            return dayFixOne
              + (l.Month == r.Month ? 0 : r.Month - l.Month)
              + (l.Year == r.Year ? 0 : (r.Year - l.Year) * 12);
        }

        /// <summary>
        /// Compute dateTime difference
        /// Alex-LEWIS, 2015-08-11
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static int GetMonthDiff(this DateTime dt1, DateTime dt2)
        {
            var l = dt1 < dt2 ? dt1 : dt2;
            var r = dt1 >= dt2 ? dt1 : dt2;
            return (l.Day == r.Day ? 0 : l.Day > r.Day ? 0 : 1)
              + (l.Month == r.Month ? 0 : r.Month - l.Month)
              + (l.Year == r.Year ? 0 : (r.Year - l.Year) * 12);
        }

        #endregion




        public static DateTime BeginningOfLastMonth(this DateTime dateTime)
        {
            DateTime newDate = dateTime.BeginningOfTheMonth();
            newDate = newDate.AddDays(-1);
            newDate = newDate.BeginningOfTheMonth();
            newDate = newDate.BeginningOfTheDay();
            return newDate;
        }

        public static DateTime EndOfLastMonth(this DateTime dateTime)
        {
            DateTime newDate = dateTime.BeginningOfTheMonth();
            newDate = newDate.AddDays(-1);
            newDate = newDate.EndOfTheMonth();
            newDate = newDate.EndOfTheDay();
            return newDate;
        }

        public static DateTime FirstDayOfNextMonth(this DateTime dateTime)
        {
            DateTime newDate = dateTime.EndOfTheMonth();
            newDate = newDate.AddDays(1);
            newDate = newDate.BeginningOfTheMonth();
            newDate = newDate.BeginningOfTheDay();
            return newDate;
        }

        public static DateTime LastDayOfNextMonth(this DateTime dateTime)
        {
            DateTime newDate = dateTime.EndOfTheMonth();
            newDate = newDate.AddDays(1);
            newDate = newDate.EndOfTheMonth();
            newDate = newDate.EndOfTheDay();
            return newDate;
        }

        public static List<DateTime> MonthRangeUpto(this DateTime startDate, DateTime endDate, bool startDateIsInclusive = true)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            List<DateTime> dates = new List<DateTime>();

            if (startDate > endDate) return dates;

            DateTime current = startDateIsInclusive ? startDate : startDate.AddMonths(1);
            while (current < endDate)
            {
                dates.Add(current);
                current = current.AddMonths(1);
            }
            return dates;
        }

        public static List<DateTime> FirstDaysOfNextMonths(this DateTime startDate, int count, bool startDateIsInclusive = true)
        {
            DateTime current = startDateIsInclusive ? startDate.BeginningOfTheMonth() : startDate.FirstDayOfNextMonth();

            List<DateTime> dates = new List<DateTime>();
            for (int i = 0; i < count; i++)
            {
                dates.Add(current);
                current = current.FirstDayOfNextMonth();
            }
            return dates;
        }

        public static List<DateTime> LastDaysOfNextMonths(this DateTime startDate, int count, bool startDateIsInclusive = true)
        {
            DateTime current = startDateIsInclusive ? startDate.EndOfTheMonth() : startDate.LastDayOfNextMonth();

            List<DateTime> dates = new List<DateTime>();
            for (int i = 0; i < count; i++)
            {
                dates.Add(current);
                current = current.LastDayOfNextMonth();
            }
            return dates;
        }

        public static List<DateTime> FirstDaysOfLastMonths(this DateTime startDate, int count, bool startDateIsInclusive = true)
        {
            DateTime current = startDateIsInclusive ? startDate.BeginningOfTheMonth() : startDate.BeginningOfLastMonth();

            List<DateTime> dates = new List<DateTime>();
            for (int i = 0; i < count; i++)
            {
                dates.Add(current);
                current = current.BeginningOfLastMonth();
            }
            dates.Reverse();
            return dates;
        }

        public static List<DateTime> LastDaysOfLastMonths(this DateTime startDate, int count, bool startDateIsInclusive = true)
        {
            DateTime current = startDateIsInclusive ? startDate.EndOfTheMonth() : startDate.EndOfLastMonth();

            List<DateTime> dates = new List<DateTime>();
            for (int i = 0; i < count; i++)
            {
                dates.Add(current);
                current = current.EndOfLastMonth();
            }
            dates.Reverse();
            return dates;
        }

        //public static DateTime SameDateNextMonth(this DateTime dateTime)
        //{
        //    DateTime newDate = dateTime.AddMonths(1).Date;
        //    return newDate;
        //}

        public static DateTime SameDateLastMonth(this DateTime dateTime)
        {
            DateTime newDate = dateTime.AddMonths(-1).Date;
            return newDate;
        }

        public static DateTime SameDateNextYear(this DateTime dateTime)
        {
            DateTime newDate = dateTime.AddYears(1).Date;
            return newDate;
        }

        public static DateTime SameDateLastYear(this DateTime dateTime)
        {
            DateTime newDate = dateTime.AddYears(-1).Date;
            return newDate;
        }

        public static List<DateTime> SameDateNextYears(this DateTime startDate, int count, bool startDateIsInclusive = true)
        {
            DateTime current = startDateIsInclusive ? startDate.Date : startDate.SameDateNextYear();

            List<DateTime> dates = new List<DateTime>();
            for (int i = 0; i < count; i++)
            {
                dates.Add(current);
                current = current.SameDateNextYear();
            }
            return dates;
        }


        // Business Days Calculation
        // http://stackoverflow.com/questions/1617049/calculate-the-number-of-business-days-between-two-dates

        /// <summary>
        /// Calculates number of business days, taking into account:
        ///  - weekends (Saturdays and Sundays)
        ///  - holidays in the middle of the week
        /// </summary>
        /// <param name="start">First day in the time interval</param>
        /// <param name="end">Last day in the time interval</param>
        /// <param name="holidays">List of holidays</param>
        /// <returns>Number of business days during the 'span'</returns>
        public static int CountBusinessDaysUntil(this DateTime start, DateTime end, params DateTime[] holidays)
        {
            if (start > end)
                throw new ArgumentException("Start date cannot be greater than end date.");

            int businessDays = CountBusinessDaysUntil1(start, end);
            //int businessDays = CountBusinessDaysUntil2(start, end);
            //int businessDays = CountBusinessDaysUntil3(start, end);

            // subtract the number of holidays during the time interval
            foreach (DateTime holiday in holidays)
            {
                DateTime bh = holiday.Date;

                // if holiday happens to be a weekend, don't subtract it because it is already accounted for.
                if (bh.DayOfWeek == DayOfWeek.Saturday || bh.DayOfWeek == DayOfWeek.Sunday) continue;

                if (start <= bh && bh <= end)
                    --businessDays;
            }

            return businessDays;
        }

        public static int CountBusinessDaysUntil3(DateTime start, DateTime end)
        {
            List<DateTime> range = start.GetDateRangeInclusiveTo(end).ToList();
            int businessDays = range.Where(day => day.IsWeekday())
                .Count();
            return businessDays;
        }

        public static int CountBusinessDaysUntil2(DateTime firstDay, DateTime lastDay)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                //int firstDayOfWeek = (int)firstDay.DayOfWeek;
                //int lastDayOfWeek = (int)lastDay.DayOfWeek;

                ///The code above assumes that DayOfWeek.Sunday has the value 7 which is not the case. 
                ///The value is actually 0. It leads to a wrong calculation if for example firstDay and 
                ///lastDay are both the same Sunday. The method returns 1 in this case but it should be 0.
                ///Easiest fix for this bug: Replace in the code above the lines where firstDayOfWeek and 
                ///lastDayOfWeek are declared by the following:
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;

                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;

                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;
            return businessDays;
        }

        public static int CountBusinessDaysUntil1(DateTime start, DateTime end)
        {
            double calcBusinessDays = 1 + ((end - start).TotalDays * 5 -
                (start.DayOfWeek - end.DayOfWeek) * 2) / 7;

            if ((int)end.DayOfWeek == 6) calcBusinessDays--;
            if ((int)start.DayOfWeek == 0) calcBusinessDays--;

            return (int)calcBusinessDays;
        }

        public static int CountWeekendDaysUntil(this DateTime start, DateTime end, params DateTime[] holidays)
        {
            if (start > end)
                throw new ArgumentException("Start date cannot be greater than end date.");

            //int weekendDays = CountWeekendDays1(start, end);
            int weekendDays = CountWeekendDays2(start, end);
            //int weekendDays = CountWeekendDays3(start, end);

            // add the number of holidays during the time interval
            foreach (DateTime holiday in holidays)
            {
                DateTime bh = holiday.Date;

                // if holiday happens to be a weekend, don't subtract it because it is already accounted for.
                if (bh.DayOfWeek == DayOfWeek.Saturday || bh.DayOfWeek == DayOfWeek.Sunday) continue;

                if (start <= bh && bh <= end)
                    ++weekendDays;
            }

            return weekendDays;
        }

        public static int CountWeekendDays3(DateTime start, DateTime end)
        {
            List<DateTime> range = start.GetDateRangeInclusiveTo(end).ToList();
            int businessDays = range.Where(day => day.IsWeekend())
                .Count();
            return businessDays;
        }

        public static int CountWeekendDays2(DateTime firstDay, DateTime lastDay)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;

            int weekendDays = 0;

            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                //int firstDayOfWeek = (int)firstDay.DayOfWeek;
                //int lastDayOfWeek = (int)lastDay.DayOfWeek;

                ///The code above assumes that DayOfWeek.Sunday has the value 7 which is not the case. 
                ///The value is actually 0. It leads to a wrong calculation if for example firstDay and 
                ///lastDay are both the same Sunday. The method returns 1 in this case but it should be 0.
                ///Easiest fix for this bug: Replace in the code above the lines where firstDayOfWeek and 
                ///lastDayOfWeek are declared by the following:
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;

                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;

                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        weekendDays += 2; //businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        weekendDays += 1;//businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    weekendDays += 1;//businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            //businessDays -= fullWeekCount + fullWeekCount;
            //return businessDays;

            weekendDays += fullWeekCount + fullWeekCount;
            return weekendDays;
        }

        public static int CountWeekendDays1(DateTime start, DateTime end)
        {
            double calcBusinessDays = 1 + ((end - start).TotalDays * 5 -
                (start.DayOfWeek - end.DayOfWeek) * 2) / 7;

            if ((int)end.DayOfWeek == 6) calcBusinessDays--;
            if ((int)start.DayOfWeek == 0) calcBusinessDays--;

            return (int)calcBusinessDays;
        }
    }
}
