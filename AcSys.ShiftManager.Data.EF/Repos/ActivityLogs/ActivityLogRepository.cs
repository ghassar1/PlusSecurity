using System.Threading.Tasks;
using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Model.Helpers;

namespace AcSys.ShiftManager.Data.EF.Repos.ActivityLogs
{
    public class ActivityLogRepository : GenericRepository<ApplicationDbContext, ActivityLog>, IActivityLogRepository
    {
        public ApplicationDbContext Context { get; set; }

        public ActivityLogRepository(ApplicationDbContext context)
            : base(context)
        {
            Context = context;
        }

        public async Task<ActivityLog> GetLastActivity(User user, Enums.ActivityType activityType)
        {
            ActivityLog activityLog = await this.FirstOrDefaultAsync(o =>
                o.User.Id == user.Id
                && o.Type == activityType,
                o => o.DateTimeStamp.Value, true);
            return activityLog;
        }

        public async Task<ActivityLog> GetLastLoginActivity(User user)
        {
            ActivityLog activityLog = await this.GetLastActivity(user, Enums.ActivityType.Login);
            return activityLog;
        }
    }
}
