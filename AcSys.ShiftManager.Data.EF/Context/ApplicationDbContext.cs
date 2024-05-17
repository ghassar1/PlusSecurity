using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AcSys.Core.Data.EF.Context;
using AcSys.ShiftManager.Data.EF.Mappings;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Context
{
    public class ApplicationDbContext : AcSysIdentityContext<User, Role, UserRole, UserClaim, UserLogin, RoleMap, UserMap, UserRoleMap, UserClaimMap, UserLoginMap>
    {
        public ApplicationDbContext()
            : base("name=MainConnection")
        //: base("data source=.;initial catalog=AcSys.ShiftManager;integrated security=True;")
        {
            
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public static void Initialize()
        {
            try
            {
                //Database.SetInitializer(new MigrateDatabaseToLatestVersion<AwDbContext, Configuration>());
                //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AwDbContext>());

                using (var transactionScope = new TransactionScope())
                {
                    using (var entities = new ApplicationDbContext())
                    {
                        entities.Database.Initialize(true);

                        // force a quick read of the database, to 'prime' it ready for use
                        var auser = entities.Users.FirstOrDefault();

                        entities.SaveChanges();
                        transactionScope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Configurations.Add(new EmployeeGroupMap());
            modelBuilder.Configurations.Add(new ActivityLogMap());

            modelBuilder.Configurations.Add(new NotificationMap());
            modelBuilder.Configurations.Add(new NotificationViewMap());

            modelBuilder.Configurations.Add(new MessageMap());
            modelBuilder.Configurations.Add(new MessageViewMap());

            modelBuilder.Configurations.Add(new ShiftMap());
        }

        public DbSet<EmployeeGroup> EmployeeGroups { get; set; }
        
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationView> NotificationViews { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageView> MessageViews { get; set; }

        public DbSet<Shift> Shifts { get; set; }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
