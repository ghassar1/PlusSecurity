using System;
using AcSys.ShiftManager.Model.Helpers;
using AcSys.ShiftManager.Service.Common;
using AcSys.ShiftManager.Service.Users;

namespace AcSys.ShiftManager.Service.ActivityLogs
{
    public class ActivityLogListItemDto : EntityDto
    {
        public UserDto User { get; set; }

        public DateTime? DateTimeStamp { get; set; }

        public Enums.ActivityType Type { get; set; }
        public string TypeDesc { get; set; }

        public string Description { get; set; }


        public Guid? SubjectId { get; set; }
        public Enums.SubjectType SubjectType { get; set; }
        public string SubjectTypeDesc { get; set; }
    }
}
