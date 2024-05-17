using System;
using AcSys.ShiftManager.Model.Helpers;
using AcSys.ShiftManager.Service.Common;

namespace AcSys.ShiftManager.Service.Shifts
{
    public class ShiftBasicDetailsDto : EntityDto
    {
        public bool IsOpen { get; set; }

        public string Title { get; set; }
        public string Notes { get; set; }

        //public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public DateTime? ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }

        public Enums.ShiftStatus Status { get; set; }
        public string StatusDesc { get; set; }

        public Enums.ShiftStartStatus StartStatus { get; set; }
        public string StartStatusDesc { get; set; }

        public Enums.ShiftEndStatus EndStatus { get; set; }
        public string EndStatusDesc { get; set; }
    }
}
