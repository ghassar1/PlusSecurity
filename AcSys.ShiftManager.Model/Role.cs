using System;
using AcSys.Core.Data.Identity.Model;
using AcSys.Core.Extensions;

namespace AcSys.ShiftManager.Model
{
    public class Role : AcSysRole<UserRole>
    {
        // TODO: Enabling following line creates the RoleUser associative table in addition to the UserRole table
        //private ICollection<User> _users = new Collection<User>();
        //public virtual ICollection<User> Users
        //{
        //    get { return _users; }
        //    set { _users = value; }
        //}

        //ICollection<Notification> _notifications = new Collection<Notification>();
        //public virtual ICollection<Notification> Notifications
        //{
        //    get { return _notifications; }
        //    set { _notifications = value; }
        //}

        public override string ToString()
        {
            //return base.ToString();
            return "{0}".FormatWith(Name);
        }

        public override string ToDescription()
        {
            return ToString();
        }

        //public Notification SendNotification(User sender, string subject, string body)
        //{
        //    Notification notification = new Notification()
        //    {
        //        Sender = sender,

        //        Title=subject,
        //         Type= Helpers.Enums.NotificationType.NewMessage
        //        Text = body,
        //        SentAt = DateTime.Now
        //    };
        //    notification.Recipients.Add(this);
        //    return notification;
        //}
    }
}
