using System;

namespace AcSys.ShiftManager.Service.Shifts
{
    public class AttendanceReportRowDto
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public DateTime Date { get; set; }
        public string Timings { get; set; }
        public string Attendance { get; set; }
        public int LateMins { get; set; }
    }
}
