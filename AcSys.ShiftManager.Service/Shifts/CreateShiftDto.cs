using System;
using System.Collections.Generic;
using AcSys.ShiftManager.Service.Common;

namespace AcSys.ShiftManager.Service.Shifts
{
    public class CreateShiftDto : EntityDto
    {
        //public bool IsOpen { get; set; }
        public List<EntityDto> Employees { get; set; }
        public List<EntityDto> Groups { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int[] Days { get; set; }
        public int? ShiftsPerDay { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public int TotalBreakMins { get; set; }

        public string Title { get; set; }

        public string Notes { get; set; }
    }
}
