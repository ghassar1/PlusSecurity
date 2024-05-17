using AcSys.Core.Data.Identity.Mappings;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Mappings
{
    public class RoleMap : AcSysRoleMap<Role, UserRole>
    {
        public RoleMap()
        {
            // TODO: Enabling following line requires enabling Users collection on Role which creates the RoleUser associative table in addition to the UserRole table
            //this.HasMany(e => e.Users)
            //    .WithMany(e => e.Roles)
            //    ;//.Map(c =>
            //     //{
            //     //    c.MapLeftKey("User_Id");
            //     //    c.MapRightKey("Role_Id");
            //     //    c.ToTable("UserRole");
            //     //});
        }
    }
}
