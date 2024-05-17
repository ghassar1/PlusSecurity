using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Data.Users;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Repos.Users
{
    public class EmployeeGroupRepository : GenericRepository<ApplicationDbContext, EmployeeGroup>, IEmployeeGroupRepository
    {
        public ApplicationDbContext Context { get; set; }

        public EmployeeGroupRepository(ApplicationDbContext context)
            : base(context)
        {
            Context = context;
        }
    }
}
