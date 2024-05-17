using System;

namespace AcSys.ShiftManager.Service.Shifts
{
    public class AttendanceSummaryReportDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public int TotalShifts { get; set; }
        public int DueShifts { get; set; }
        public int CompletedShifts { get; set; }
        public int LateShifts { get; set; }

        public int TotalEmployees { get; set; }

        public int TotalHours { get; set; }
        public int TotalClockedHours { get; set; }

        public int AttendancePercentage { get; set; }
        public int LateAttendancePercentage { get; set; }
    }
}
