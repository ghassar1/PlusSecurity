using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Data.Notifications;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Repos.Notifications
{
    public class NotificationViewRepository : GenericRepository<ApplicationDbContext, NotificationView>, INotificationViewRepository
    {
        public ApplicationDbContext Context { get; set; }

        public NotificationViewRepository(ApplicationDbContext context)
            : base(context)
        {
            Context = context;
        }
    }
}
