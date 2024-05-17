using System;
using AcSys.ShiftManager.Model;
using Microsoft.AspNet.Identity;

namespace AcSys.ShiftManager.Data.EF.Identity
{
    public class ApplicationRoleManager : RoleManager<Role, Guid>
    {
        public ApplicationRoleManager(IRoleStore<Role, Guid> store)
            : base(store)
        {
            
        }
    }
}
