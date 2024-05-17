using System.Data.Entity.ModelConfiguration;
using AcSys.Core.Data.EF.Mappings.Base;
using AcSys.Core.Data.Identity.Model;

namespace AcSys.Core.Data.Identity.Mappings
{
    public class AcSysUserRoleMap<TUserRole> : EntityTypeConfiguration<TUserRole>//EntityMapBase<TUserRole>
        where TUserRole : AcSysUserRole
    {
        public AcSysUserRoleMap()
        {
            ToTable("UserRole");

            HasKey(e => new { e.RoleId, e.UserId });
        }
    }
}
