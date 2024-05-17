using AcSys.Core.Data.EF.Mappings.Base;
using AcSys.Core.Data.Identity.Model;

namespace AcSys.Core.Data.Identity.Mappings
{
    public class AcSysUserClaimMap<TUserClaim> : EntityMapBase<TUserClaim>
        where TUserClaim : AcSysUserClaim
    {
        public AcSysUserClaimMap()
        {
            this.ToTable("UserClaim");

            this.Property(e => e.ClaimType)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(e => e.ClaimValue)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
