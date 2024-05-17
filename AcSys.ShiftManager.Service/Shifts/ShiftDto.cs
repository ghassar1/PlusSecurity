using AcSys.ShiftManager.Service.Users;
using System;

namespace AcSys.ShiftManager.Service.Shifts
{
    public class ShiftDto : ShiftBasicDetailsDto
    {
        public UserDto Employee { get; set; }

        public int TotalBreakMins { get; set; }
        public int TotalMins { get; set; }
        public int LateMins { get; set; }
        public int ClockedMins { get; set; }
        public int ShortMins { get; set; }

        //public string Notes { get; set; }

        public DateTime Now { get; set; }
        public DateTime NowUtc { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public TimeZone TimeZone { get; set; }
    }
}
