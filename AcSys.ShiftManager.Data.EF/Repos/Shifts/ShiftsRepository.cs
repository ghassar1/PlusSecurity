using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Data.Shifts;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Repos.Shifts
{
    public class ShiftRepository : GenericRepository<ApplicationDbContext, Shift>, IShiftRepository
    {
        public ApplicationDbContext Context { get; set; }

        public ShiftRepository(ApplicationDbContext context)
            : base(context)
        {
            Context = context;
        }
    }
}
