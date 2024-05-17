using System.Threading.Tasks;
using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Model.Helpers;

namespace AcSys.ShiftManager.Data
{
    public interface IActivityLogRepository : IGenericRepository<ActivityLog>
    {
        Task<ActivityLog> GetLastActivity(User user, Enums.ActivityType activityType);
        
        Task<ActivityLog> GetLastLoginActivity(User user);
    }
}
