using System;
using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Model;
using Microsoft.AspNet.Identity;

namespace AcSys.ShiftManager.Data.Users
{
    public interface IRoleRepository 
        : IGenericRepository<Role>,
        IQueryableRoleStore<Role, Guid>,
        IRoleStore<Role, Guid>
    {

    }
}
