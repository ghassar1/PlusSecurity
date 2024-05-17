using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AcSys.Core.Data.Identity.Mappings;
using AcSys.Core.Data.Identity.Model;
using AcSys.Core.Data.Model.Base;
using AcSys.Core.Extensions;

namespace AcSys.Core.Data.EF.Context
{
    public class AcSysIdentityContext<TUser, TRole, TUserRole, TUserClaim, TUserLogin, TRoleMap, TUserMap, TUserRoleMap, TUserClaimMap, TUserLoginMap> : AcSysContext
        where TUser : AcSysUser<TRole, TUserRole, TUserClaim, TUserLogin>
        where TRole : AcSysRole<TUserRole>
        where TUserClaim : AcSysUserClaim
        where TUserRole : AcSysUserRole
        where TUserLogin : AcSysUserLogin
        where TRoleMap : AcSysRoleMap<TRole, TUserRole>, new()
        where TUserMap : AcSysUserMap<TUser, TRole, TUserRole, TUserClaim, TUserLogin>, new()
        where TUserRoleMap : AcSysUserRoleMap<TUserRole>, new()
        where TUserClaimMap : AcSysUserClaimMap<TUserClaim>, new()
        where TUserLoginMap : AcSysUserLoginMap<TUserLogin>, new()
    {
        public AcSysIdentityContext()
            : base("name=MainConnection")
        {
        }

        public AcSysIdentityContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Configurations.Add(new TRoleMap());
            modelBuilder.Configurations.Add(new TUserMap());
            modelBuilder.Configurations.Add(new TUserRoleMap());
            modelBuilder.Configurations.Add(new TUserClaimMap());
            modelBuilder.Configurations.Add(new TUserLoginMap());
            
        }

        public virtual DbSet<TRole> Roles { get; set; }
        public virtual DbSet<TUser> Users { get; set; }
        public virtual DbSet<TUserRole> UserRoles { get; set; }
        public virtual DbSet<TUserClaim> UserClaims { get; set; }
        public virtual DbSet<TUserLogin> UserLogins { get; set; }

        public override int SaveChanges()
        {
            MarkEntriesWithAuditStamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync()
        {
            MarkEntriesWithAuditStamps();
            return await base.SaveChangesAsync();
        }

        void MarkEntriesWithAuditStamps()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is AuditableEntityBase
                    && (x.State == EntityState.Added || x.State == EntityState.Modified));

            if (modifiedEntries.Count() > 0)
            {
                string username = string.Empty;
                string identityName = Thread.CurrentPrincipal.Identity.Name.ToUpper();
                if (!string.IsNullOrWhiteSpace(identityName))
                {
                    TUser user = this.Users.FirstOrDefault(u => u.UserName.ToUpper() == identityName || u.Email == identityName);
                    username = user == null ? identityName : user.Email;
                }

                foreach (var entry in modifiedEntries)
                {
                    AuditableEntityBase auditableEntity = entry.Entity as AuditableEntityBase;
                    if (auditableEntity != null)
                    {
                        DateTime now = DateTime.UtcNow;

                        if (entry.State == EntityState.Added)
                        {
                            auditableEntity.CreatedByUser = username;
                            auditableEntity.CreatedDate = now;
                        }
                        else
                        {
                            base.Entry(auditableEntity).Property(x => x.CreatedByUser).IsModified = false;
                            base.Entry(auditableEntity).Property(x => x.CreatedDate).IsModified = false;
                        }

                        auditableEntity.UpdatedByUser = username;
                        auditableEntity.UpdatedDate = now;

                        if (entry.State == EntityState.Modified)
                        {
                            Console.WriteLine("Original Values");
                            foreach (string prop in entry.OriginalValues.PropertyNames)
                            {
                                string original = entry.OriginalValues[prop] == null ? "" : entry.OriginalValues[prop].ToString();
                                string current = entry.CurrentValues[prop] == null ? "" : entry.CurrentValues[prop].ToString();

                                if (original != current)
                                {
                                    Console.WriteLine("{0}: {1}  =>  {2}".FormatWith(prop, original, current));
                                }
                            }
                        }
                    }

                    IEntity entity = entry.Entity as IEntity;
                    if (entity != null)
                    {
                        //entity.Validate();
                    }
                }
            }
        }
    }
}
