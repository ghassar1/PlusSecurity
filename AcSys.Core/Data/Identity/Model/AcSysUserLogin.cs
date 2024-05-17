using System;
using AcSys.Core.Data.Model.Base;

namespace AcSys.Core.Data.Identity.Model
{
    public class AcSysUserLogin : EntityBase
    {
        public virtual Guid UserId { get; set; }

        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }
    }
}
