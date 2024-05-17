using AcSys.Core.Data.EF.Mappings.Base;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Mappings
{
    public class ActivityLogMap : EntityMapBase<ActivityLog>
    {
        public ActivityLogMap()
        {
            HasRequired(e => e.User)
                .WithMany(a => a.ActivityLogs);

            Property(e => e.Type)
                .IsRequired();

            Property(e => e.Description)
                .IsRequired();

            Property(e => e.DateTimeStamp)
                .IsRequired();

            Property(e => e.SubjectType)
                .IsRequired();

            Property(e => e.SubjectId)
                .IsOptional();

            Property(e => e.SubjectSnapshot)
                .IsOptional();
        }
    }
}
