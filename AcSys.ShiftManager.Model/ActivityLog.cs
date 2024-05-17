using System;
using AcSys.Core.Data.Model.Base;
using AcSys.ShiftManager.Model.Helpers;

namespace AcSys.ShiftManager.Model
{
    public class ActivityLog : EntityBase
    {
        public ActivityLog() { }

        public ActivityLog(User user,
            Enums.ActivityType type,
            string desc,
            Enums.SubjectType subjectType,
            IEntity subject)
        {
            _user = user;

            Type = type;
            Description = desc;

            SubjectType = subjectType;
            SubjectId = subject.Id;
            //SubjectDesc = subject.ToString();
            SubjectSnapshot = subject.ToString();

            DateTimeStamp = DateTime.Now;
        }

        User _user = null;
        public virtual User User
        {
            get { return _user; }
            set { _user = value; }
        }

        public DateTime? DateTimeStamp { get; set; }

        public Enums.ActivityType Type { get; set; }
        public string Description { get; set; }

        public Enums.SubjectType SubjectType { get; set; }
        public Guid? SubjectId { get; set; }
        //public string SubjectDesc { get; set; }
        public string SubjectSnapshot { get; set; }
    }
}
