using System.Data.Entity.ModelConfiguration;
using AcSys.Core.Data.Identity.Model;

namespace AcSys.Core.Data.Identity.Mappings
{
    public class AcSysUserLoginMap<TUserLogin> : EntityTypeConfiguration<TUserLogin> //EntityMapBase<UserLogin>
        where TUserLogin : AcSysUserLogin
    {
        public AcSysUserLoginMap()
        {
            this.ToTable("UserLogin");

            this.HasKey(u => new { u.UserId, u.LoginProvider, u.ProviderKey });

            //this.Property(e => e.UserId)
            //    .IsRequired()
            //    .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(
            //        new IndexAttribute("IX_UniqueUserLogin", 1)
            //        {
            //            IsUnique = true
            //        }));

            //this.Property(e => e.LoginProvider)
            //    .IsRequired()
            //    .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(
            //        new IndexAttribute("IX_UniqueUserLogin", 2)
            //        {
            //            IsUnique = true
            //        }));

            //this.Property(e => e.ProviderKey)
            //    .IsRequired()
            //    .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(
            //        new IndexAttribute("IX_UniqueUserLogin", 3)
            //        {
            //            IsUnique = true
            //        }));
        }
    }
}
