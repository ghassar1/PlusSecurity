using System.ComponentModel;

namespace AcSys.ShiftManager.Model.Helpers
{
    public static class Enums
    {
        public enum ActivityType
        {
            [Description("Login")]
            Login = 0,

            [Description("Logout")]
            Logout = 1,

            [Description("Created")]
            Created = 2,

            [Description("Updated")]
            Updated = 3,

            [Description("Deleted")]
            Deleted = 4,

            [Description("Viewed")]
            Viewed = 5,

            [Description("Searched")]
            Searched = 6,

            [Description("Sent")]
            Sent = 7,

            [Description("Read")]
            Read = 8,

            [Description("Marked As Unread")]
            MarkedAsUnread = 9,

            [Description("Activated")]
            Activated = 10,

            [Description("Deactivated")]
            Deactivated = 11,

            [Description("Shift Taken")]
            ShiftTaken = 12,

            [Description("Shift Left")]
            ShiftLeft = 13,

            [Description("ClockIn")]
            ClockIn = 14,

            [Description("ClockOut")]
            ClockOut = 15,
        }

        public enum SubjectType
        {
            [Description("None")]
            None = 0,

            [Description("User")]
            User = 1,

            [Description("Employee Group")]
            EmployeeGroup = 2,

            [Description("Message")]
            Message = 3,

            [Description("Notification")]
            Notification = 4,

            [Description("Shift")]
            Shift = 5
        }

        //public enum NotificationSource
        //{
        //    [Description("Activity")]
        //    Activity = 0,

        //    [Description("User")]
        //    User = 1
        //}

        public enum NotificationType
        {
            [Description("User Notification")]
            UserNotification = 0,

            [Description("New Message")]
            NewMessage = 1,

            [Description("New Shift")]
            NewShift = 2
        }

        public enum ShiftStatus
        {
            [Description("Due")]
            Due = 0,

            [Description("Complete")]
            Complete = 1,

            [Description("Missed")]
            Missed = 2

            //[Description("Short")]
            //Short = 3,

            //[Description("Clocked-In")]
            //ClockedIn = 4
        }

        public enum ShiftStartStatus
        {
            [Description("None")]
            None = 0,

            [Description("On Time")]
            OnTime = 1,

            [Description("Late")]
            Late = 2
        }

        public enum ShiftEndStatus
        {
            [Description("None")]
            None = 0,

            [Description("Early")]
            Early = 1,

            [Description("On Time")]
            OnTime = 2,

            //[Description("Late")]
            //Late = 3
        }
    }
}
