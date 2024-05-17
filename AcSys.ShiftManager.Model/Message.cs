using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcSys.Core.Data.Model.Base;

namespace AcSys.ShiftManager.Model
{
    public class Message : EntityBase
    {
        public string Subject { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }

        public virtual User Sender { get; set; }
        
        private ICollection<User> _recipients = new Collection<User>();
        public virtual ICollection<User> Recipients
        {
            get { return _recipients; }
            set { _recipients = value; }
        }
        
        private ICollection<MessageView> _views = new Collection<MessageView>();
        public virtual ICollection<MessageView> Views
        {
            get { return _views; }
            set { _views = value; }
        }
    }
}
