using AcSys.Core.Data.Identity.Mappings;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Mappings
{
    public class UserRoleMap : AcSysUserRoleMap<UserRole>
    {
        public UserRoleMap()
        {
            HasRequired(e => e.Role)
                .WithMany(e => e.UserRoles)
                .HasForeignKey(e => e.RoleId);

            HasRequired(e => e.User)
                .WithMany(e => e.UserRoles)
                .HasForeignKey(e => e.UserId);
        }
    }
}
