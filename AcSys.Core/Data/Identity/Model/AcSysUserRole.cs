using System;

namespace AcSys.Core.Data.Identity.Model
{
    public class AcSysUserRole// : EntityBase
    {
        public virtual Guid RoleId { get; set; }

        public virtual Guid UserId { get; set; }
    }
}
