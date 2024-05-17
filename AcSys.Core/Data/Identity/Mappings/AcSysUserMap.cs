using AcSys.Core.Data.EF.Mappings.Base;
using AcSys.Core.Data.Identity.Model;

namespace AcSys.Core.Data.Identity.Mappings
{
    public class AcSysUserMap<TUser, TRole, TUserRole, TUserClaim, TUserLogin> : EntityMapBase<TUser>
        where TUser : AcSysUser<TRole, TUserRole, TUserClaim, TUserLogin>
        where TRole : AcSysRole<TUserRole>
        where TUserRole : AcSysUserRole
        where TUserClaim : AcSysUserClaim
        where TUserLogin : AcSysUserLogin
    {
        public AcSysUserMap()
        {
            this.ToTable("User");
            //this.MapToStoredProcedures();

            this.HasMany(e => e.UserRoles);
            
            this.HasMany(e => e.Claims);

            this.HasMany(e => e.Logins);

            this.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(25);

            this.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(25);

            this.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(75);

            this.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(75);

            this.Property(e => e.PasswordHash)
                .HasMaxLength(200);

            this.Property(e => e.PhoneNumber)
                .HasMaxLength(20);

            this.Property(e => e.Mobile)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(e => e.SecurityStamp)
                .HasMaxLength(200);
        }
    }
}
