using AcSys.Core.Data.Identity.Mappings;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Mappings
{
    public class UserMap : AcSysUserMap<User, Role, UserRole, UserClaim, UserLogin>
    {
        public UserMap()
        {
            // TODO: Enabling following line requires enabling Users collection on Role which creates the RoleUser associative table in addition to the UserRole table
            //HasMany(e => e.Roles)
            //    .WithMany(e => e.Users)
            //    ;//.Map(c =>
            //    //{
            //    //    c.MapLeftKey("Role_Id");
            //    //    c.MapRightKey("User_Id");
            //    //    c.ToTable("UserRole");
            //    //});

            HasMany(e => e.Claims)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId);

            HasMany(e => e.Logins)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId);

            //HasMany(e => e.Groups)
            //    .WithMany(e => e.Employees);

            HasOptional(e => e.EmployeeGroup)
                .WithMany(e => e.Employees);
        }
    }
}
