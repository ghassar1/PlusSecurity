using AcSys.Core.Data.EF.Mappings.Base;
using AcSys.Core.Data.Identity.Model;

namespace AcSys.Core.Data.Identity.Mappings
{
    public class AcSysRoleMap<TRole, TUserRole> : EntityMapBase<TRole>
        where TRole : AcSysRole<TUserRole>
        where TUserRole : AcSysUserRole
    {
        public AcSysRoleMap()
        {
            this.ToTable("Role");

            this.HasMany(e => e.UserRoles);

            this.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
