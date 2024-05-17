using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcSys.Core.Data.Model.Base;
using Microsoft.AspNet.Identity;

namespace AcSys.Core.Data.Identity.Model
{
    public class AcSysRole<TUserRole> : EntityBase, IRole<Guid>
        where TUserRole : AcSysUserRole
    {
        public AcSysRole()
        {
            userRoles = new Collection<TUserRole>();
        }

        public string Name { get; set; }

        ICollection<TUserRole> userRoles;
        public virtual ICollection<TUserRole> UserRoles
        {
            get { return userRoles; }
            set { userRoles = value; }
        }
    }
}
