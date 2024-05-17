using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AcSys.Core.Data.Identity.Model;
using AcSys.Core.Data.Model.Base;
using AcSys.Core.Extensions;
using AcSys.ShiftManager.Model.Helpers;

namespace AcSys.ShiftManager.Model
{
    public class User : AcSysUser<Role, UserRole, UserClaim, UserLogin>
    {
        public string WorkNumber { get; set; }
        public string Location { get; set; }

        public string Address { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }

        public bool HasDrivingLicense { get; set; }

        //ICollection<Role> roles = new Collection<Role>();
        //public override ICollection<Role> Roles
        //{
        //    get { return roles; }
        //    set { roles = value; }
        //}

        public virtual EmployeeGroup EmployeeGroup { get; set; }

        //ICollection<EmployeeGroup> groups = new Collection<EmployeeGroup>();
        //public virtual ICollection<EmployeeGroup> Groups
        //{
        //    get { return groups; }
        //    set { groups = value; }
        //}

        ICollection<ActivityLog> activityLogs = new Collection<ActivityLog>();
        public virtual ICollection<ActivityLog> ActivityLogs
        {
            get { return activityLogs; }
            set { activityLogs = value; }
        }

        ICollection<Notification> authoredNotifications = new Collection<Notification>();
        public virtual ICollection<Notification> AuthoredNotifications
        {
            get { return authoredNotifications; }
            set { authoredNotifications = value; }
        }

        ICollection<Notification> _notifications = new Collection<Notification>();
        public virtual ICollection<Notification> Notifications
        {
            get { return _notifications; }
            set { _notifications = value; }
        }

        ICollection<NotificationView> notificationViews = new Collection<NotificationView>();
        public virtual ICollection<NotificationView> NotificationViews
        {
            get { return notificationViews; }
            set { notificationViews = value; }
        }

        ICollection<Message> outgoingMessages = new Collection<Message>();
        public virtual ICollection<Message> OutgoingMessages
        {
            get { return outgoingMessages; }
            set { outgoingMessages = value; }
        }

        ICollection<Message> incomingMessages = new Collection<Message>();
        public virtual ICollection<Message> IncomingMessages
        {
            get { return incomingMessages; }
            set { incomingMessages = value; }
        }

        ICollection<MessageView> messageViews = new Collection<MessageView>();
        public virtual ICollection<MessageView> MessageViews
        {
            get { return messageViews; }
            set { messageViews = value; }
        }

        ICollection<Shift> shifts = new Collection<Shift>();
        public virtual ICollection<Shift> Shifts
        {
            get { return shifts; }
            set { shifts = value; }
        }

        public override string ToString()
        {
            //return base.ToString();
            return "{0} {1}".FormatWith(FirstName, LastName);
        }

        public override string ToDescription()
        {
            return ToString();
        }

        public Role GetRole()
        {
            //Role role = Roles.FirstOrDefault();
            //Role role = UserRoles.FirstOrDefault().Role;
            //if (role == null) throw new ApplicationException("User has no associated role.");
            //UserRole userRole = UserRoles.FirstOrDefault();
            //return userRole == null ? null : userRole.Role;
            //if (userRole == null) throw new ApplicationException("User has no associated role.");
            //return userRole.Role;
            Role role = UserRoles.Select(r => r.Role).FirstOrDefault();
            //if (role == null) throw new ApplicationException("User has no associated role.");
            return role;
        }

        public Role GetRole(Guid roleId)
        {
            //UserRole userRole = UserRoles.FirstOrDefault(r => r.Role.Id == roleId);
            //if (userRole == null) throw new ApplicationException("User has no role with this id.");
            //return userRole.Role;

            Role role = UserRoles.Select(r => r.Role).FirstOrDefault(r => r.Id == roleId);
            if (role == null) throw new ApplicationException("User has no role with this id.");
            return role;
        }

        public Role GetRole(string roleName)
        {
            //UserRole userRole = UserRoles.FirstOrDefault(r => r.Role.Name.ToUpper() == roleName.ToUpper());
            //if (userRole == null) throw new ApplicationException("User has no role with this name.");
            //return userRole.Role;

            Role role = UserRoles.Select(r => r.Role).FirstOrDefault(r => r.Name.ToUpper() == roleName.ToUpper());
            if (role == null) throw new ApplicationException("User has no role with this name.");
            return role;
        }

        public ActivityLog AddLog(Enums.SubjectType subjectType, Enums.ActivityType activityType, string activityDesc = "")
        {
            return AddLog(subjectType, activityType, activityDesc, null);
        }

        public ActivityLog AddLog(Enums.ActivityType activityType, IEntity subject, string activityDesc = "")
        {
            Enums.SubjectType subjectType = GetSubjectType(subject);
            return AddLog(subjectType, activityType, activityDesc, subject);
        }

        static Enums.SubjectType GetSubjectType(IEntity subject)
        {
            Enums.SubjectType subjectType;
            if (subject is EmployeeGroup)
            {
                subjectType = Enums.SubjectType.EmployeeGroup;
            }
            else if (subject is Message)
            {
                subjectType = Enums.SubjectType.Message;
            }
            else if (subject is Notification)
            {
                subjectType = Enums.SubjectType.Notification;
            }
            else if (subject is Shift)
            {
                subjectType = Enums.SubjectType.Shift;
            }
            else if (subject is User)
            {
                subjectType = Enums.SubjectType.User;
            }
            else
            {
                subjectType = Enums.SubjectType.None;
            }

            return subjectType;
        }

        public ActivityLog AddLog(Enums.SubjectType subjectType, Enums.ActivityType activityType, string activityDesc, IEntity subject)
        {
            ActivityLog activityLog = new ActivityLog()
            {
                User = this,
                DateTimeStamp = DateTime.Now,

                SubjectType = subjectType,
                Type = activityType,
                Description = activityDesc,
            };

            if (activityDesc.IsNullOrWhiteSpace())
            {
                activityLog.Description = subjectType == Enums.SubjectType.None ? activityType.ToDescription()
                    : "{0} {1}".FormatWith(activityType.ToDescription(), subjectType.ToDescription());
            }

            if (subject != null)
            {
                activityLog.SubjectId = subject.Id;
                //activityLog.SubjectDesc = subject.ToString();

                //if (activityType == Enums.ActivityType.Updated
                //    || activityType == Enums.ActivityType.Deleted)
                //{
                //    activityLog.SubjectSnapshot = subject.ToJson();
                //}
                activityLog.SubjectSnapshot = subject.ToDescription();
            }

            ActivityLogs.Add(activityLog);
            return activityLog;
        }

        public bool HasRole(Guid roleId)
        {
            //bool hasRole = Roles.FirstOrDefault(r => r.Id == roleId) != null;
            //bool hasRole = UserRoles.FirstOrDefault(r => r.Role.Id == roleId) != null;
            //bool hasRole = UserRoles.FirstOrDefault(r => r.RoleId == roleId) != null;
            bool hasRole = UserRoles.Count(r => r.RoleId == roleId) > 0;
            return hasRole;
        }

        public bool HasRole(string roleName)
        {
            //var role = GetRole(roleName);
            //bool hasRole = Roles.FirstOrDefault(r => r.Name.ToUpper() == roleName.ToUpper()) != null;
            //bool hasRole = UserRoles.FirstOrDefault(r => r.Role.Name.ToUpper() == roleName.ToUpper()) != null;
            bool hasRole = UserRoles.Count(r => r.Role.Name.ToUpper() == roleName.ToUpper()) > 0;
            return hasRole;
        }

        public bool HasRole(Role role)
        {
            //bool hasRole = UserRoles.FirstOrDefault(o => o.RoleId == role.Id) != null;
            bool hasRole = UserRoles.Count(o => o.RoleId == role.Id) > 0;
            return hasRole;
        }

        public bool DoesNotHaveRole(Guid roleId)
        {
            return !HasRole(roleId);
        }

        public bool DoesNotHaveRole(string roleName)
        {
            return !HasRole(roleName);
        }

        public bool DoesNotHaveRole(Role role)
        {
            return !HasRole(role);
        }

        public void AddRole(Role role)
        {
            //Roles.Add(role);
            UserRoles.Add(new UserRole() { Role = role, RoleId = role.Id, User = this, UserId = Id });
        }

        public void RemoveRole(Guid roleId)
        {
            //UserRole userRole = Roles.FirstOrDefault(r => r.Id == roleId);
            UserRole userRole = UserRoles.FirstOrDefault(r => r.Role.Id == roleId);
            if (userRole == null)
                throw new InvalidOperationException("User cannot be removed from this role. User does not have this role.");

            //Roles.Remove(userRole);
            UserRoles.Remove(userRole);
        }

        public void RemoveRole(string roleName)
        {
            //var role = GetRole(roleName);
            //var userRole = Roles.FirstOrDefault(r => r.Id == role.Id);
            //var userRole = UserRoles.FirstOrDefault(r => r.Id == role.Id);
            var userRole = UserRoles.FirstOrDefault(r => r.Role.Name == roleName);
            if (userRole == null)
                throw new InvalidOperationException("User cannot be removed from this role. User does not have this role.");

            //Roles.Remove(userRole);
            UserRoles.Remove(userRole);
        }

        public void RemoveRole(Role role)
        {
            if (DoesNotHaveRole(role)) return;

            //Roles.Remove(role);
            UserRole userRole = UserRoles.FirstOrDefault(o => o.RoleId == role.Id);
            UserRoles.Remove(userRole);
        }

        public void RemoveRoles()
        {
            //Roles.Clear();
            UserRoles.Clear();
        }

        public List<string> GetRoleNames()
        {
            //var roleNames = Roles.Select(r => r.Name).ToList();
            var roleNames = UserRoles.Select(r => r.Role.Name).ToList();
            return roleNames;
        }

        public string GetFullName()
        {
            return "{0} {1}".FormatWith(FirstName, LastName);
        }

        public Message SendMessage(User sender, string subject, string body)
        {
            Message message = new Message()
            {
                Sender = sender,

                Subject = subject,
                Text = body,
                SentAt = DateTime.Now
            };
            message.Recipients.Add(this);
            return message;
        }

        public override bool HasChildRecords()
        {
            var hasData = this.ActivityLogs.Count > 0
                || this.NotificationViews.Count > 0 || this.AuthoredNotifications.Count > 0 || this.Notifications.Count > 0
                || this.MessageViews.Count > 0 || this.IncomingMessages.Count > 0 || this.OutgoingMessages.Count > 0 
                || this.Shifts.Count > 0;
            return hasData;
        }
    }
}
