using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcSys.Core.Data.Model.Base;
using AcSys.ShiftManager.Model.Helpers;

namespace AcSys.ShiftManager.Model
{
    public class Notification : EntityBase
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime SentAt { get; set; }

        public bool IsPublic { get; set; }

        //public Enums.NotificationSource Source { get; set; }
        public Enums.NotificationType Type { get; set; }
        
        public Guid SubjectId { get; set; }

        public virtual User Sender { get; set; }

        //ICollection<Role> _recipients = new Collection<Role>();
        //public virtual ICollection<Role> Recipients
        //{
        //    get { return _recipients; }
        //    set { _recipients = value; }
        //}

        ICollection<User> _recipients = new Collection<User>();
        public virtual ICollection<User> Recipients
        {
            get { return _recipients; }
            set { _recipients = value; }
        }

        ICollection<NotificationView> _views = new Collection<NotificationView>();
        public virtual ICollection<NotificationView> Views
        {
            get { return _views; }
            set { _views = value; }
        }
    }
}
