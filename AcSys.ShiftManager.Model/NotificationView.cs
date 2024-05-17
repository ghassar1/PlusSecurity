using System;
using AcSys.Core.Data.Model.Base;

namespace AcSys.ShiftManager.Model
{
    public class NotificationView : EntityBase
    {
        public virtual Notification Notification { get; set; }

        public virtual User User { get; set; }

        public DateTime ViewedAt { get; set; }
    }
}
