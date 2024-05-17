using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcSys.Core.Data.Model.Base;
using Microsoft.AspNet.Identity;

namespace AcSys.Core.Data.Identity.Model
{
    public class AcSysUser<TRole, TUserRole, TUserClaim, TUserLogin> : EntityBase, IUser<Guid>
        where TRole : AcSysRole<TUserRole>
        where TUserRole : AcSysUserRole
        where TUserClaim : AcSysUserClaim
        where TUserLogin : AcSysUserLogin
    {
        public AcSysUser()
        {
            //roles = new Collection<TRole>();
            userRoles = new Collection<TUserRole>();
            claims = new Collection<TUserClaim>();
            logins = new Collection<TUserLogin>();
        }

        #region Identity Fields

        //ICollection<TRole> roles;
        //public virtual ICollection<TRole> Roles
        //{
        //    get { return roles; }
        //    set { roles = value; }
        //}

        ICollection<TUserRole> userRoles;
        public virtual ICollection<TUserRole> UserRoles
        {
            get { return userRoles; }
            set { userRoles = value; }
        }

        ICollection<TUserClaim> claims;
        public virtual ICollection<TUserClaim> Claims
        {
            get { return claims; }
            set { claims = value; }
        }

        ICollection<TUserLogin> logins;
        public virtual ICollection<TUserLogin> Logins
        {
            get { return logins; }
            set { logins = value; }
        }

        public int AccessFailedCount { get; set; }

        public string Email { get; set; }
        
        public bool EmailConfirmed { get; set; }
        
        public bool LockoutEnabled { get; set; }
        
        public DateTime? LockoutEndDateUtc { get; set; }
        
        public string PasswordHash { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public bool PhoneNumberConfirmed { get; set; }
        
        public virtual string SecurityStamp { get; set; }

        public virtual bool TwoFactorEnabled { get; set; }

        public virtual string UserName { get; set; }

        #endregion

        public bool MustChangePassword { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Mobile { get; set; }
    }
}
