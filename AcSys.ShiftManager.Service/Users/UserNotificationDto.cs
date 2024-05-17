using System;
using AcSys.ShiftManager.Service.Common;

namespace AcSys.ShiftManager.Service.Users
{
    public class UserNotificationDto : EntityDto
    {
        public string Sender { get; set; }

        public string Title { get; set; }
        public DateTime SentAt { get; set; }

        public bool IsMessage { get; set; }
    }
}
