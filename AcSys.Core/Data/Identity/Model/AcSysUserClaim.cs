using System;
using AcSys.Core.Data.Model.Base;

namespace AcSys.Core.Data.Identity.Model
{
    public class AcSysUserClaim : EntityBase
    {
        public Guid UserId { get; set; }

        public string ClaimType { get; set; }
        
        public string ClaimValue { get; set; }
    }
}
