using System;
using AcSys.ShiftManager.Service.Common;

namespace AcSys.ShiftManager.Service.Shifts
{
    public class UpdateShiftDto : EntityDto
    {
        public EntityDto Employee { get; set; }

        public DateTime? Date { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public int TotalBreakMins { get; set; }

        public string Title { get; set; }

        public string Notes { get; set; }
    }

    public class AssignShiftDto : EntityDto
    {
        public bool IsOpen { get; set; }
        public Guid? EmployeeId { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }
    }
}
