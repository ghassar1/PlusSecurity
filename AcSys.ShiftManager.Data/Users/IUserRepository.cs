using System;
using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Model;
using Microsoft.AspNet.Identity;

namespace AcSys.ShiftManager.Data.Users
{
    public interface IUserRepository
        : IDisposable,
        IGenericRepository<User>,
        IUserStore<User, Guid>,
        IUserRoleStore<User, Guid>,
        //IUserClaimRepository,
        //IUserLoginStore<User, Guid>,
        //IUserClaimStore<User, Guid>, 
        IUserPasswordStore<User, Guid>,
        IUserSecurityStampStore<User, Guid>,
        IQueryableUserStore<User, Guid>,
        IUserEmailStore<User, Guid>
        //IUserPhoneNumberStore<User, Guid>,
        //IUserTwoFactorStore<User, Guid>,
        //IUserLockoutStore<User, Guid>
    {

    }
}
