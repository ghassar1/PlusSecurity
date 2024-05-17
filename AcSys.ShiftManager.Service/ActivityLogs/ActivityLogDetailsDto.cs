namespace AcSys.ShiftManager.Service.ActivityLogs
{
    public class ActivityLogDetailsDto : ActivityLogListItemDto
    {
        public string SubjectSnapshot { get; set; }

        public string CurrentSubjectSnapshot { get; set; }
    }
}
