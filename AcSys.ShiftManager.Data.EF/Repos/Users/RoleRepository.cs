using System;
using System.Linq;
using System.Threading.Tasks;
using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Data.Users;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Repos.Users
{
    public class RoleRepository : GenericRepository<ApplicationDbContext, Role>, IRoleRepository
    {
        public ApplicationDbContext Context { get; set; }

        public RoleRepository(ApplicationDbContext context)
            : base(context)
        {
            Context = context;
        }

        public IQueryable<Role> Roles
        {
            get
            {
                return GetAsQueryable();
            }
        }

        public async Task CreateAsync(Role role)
        {
            await base.AddAsync(role);
        }

        public async Task DeleteAsync(Role role)
        {
            base.Delete(role);
            await Context.SaveChangesAsync();
        }

        public async Task<Role> FindByIdAsync(string roleId)
        {
            return await FindAsync(Guid.Parse(roleId));
        }

        public async Task<Role> FindByIdAsync(Guid roleId)
        {
            return await FindAsync(roleId);
        }

        public async Task<Role> FindByNameAsync(string roleName)
        {
            return await FirstOrDefaultAsync(o => o.Name.ToUpper() == roleName.ToUpper());
        }

        public async Task UpdateAsync(Role role)
        {
            base.Update(role);
            await Context.SaveChangesAsync();
        }
    }
}
