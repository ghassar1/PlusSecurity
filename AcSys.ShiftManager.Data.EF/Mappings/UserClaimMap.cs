using AcSys.Core.Data.Identity.Mappings;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Mappings
{
    public class UserClaimMap : AcSysUserClaimMap<UserClaim>
    {
        public UserClaimMap()
        {
            this.HasRequired(e => e.User)
                .WithMany(e => e.Claims)
                .HasForeignKey(e => e.UserId);
        }
    }
}
