using System;
using System.Collections.Generic;
using AcSys.ShiftManager.Model.Helpers;
using AcSys.ShiftManager.Service.Common;
using AcSys.ShiftManager.Service.Users;

namespace AcSys.ShiftManager.Service.Notifications
{
    public class NotificationDto : EntityDto
    {
        public NotificationDto()
        {
            IsViewed = false;
            ViewedAt = null;
            Recipients = new List<string>();
        }

        public UserDto Sender { get; set; }

        //public Enums.NotificationSource Source { get; set; }
        public string SourceDesc { get; set; }

        public Enums.NotificationType Type { get; set; }
        public string TypeDesc { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }

        public bool IsPublic { get; set; }

        public bool IsViewed { get; set; }

        public DateTime? ViewedAt { get; set; }

        public List<string> Recipients { get; set; }
    }
}
