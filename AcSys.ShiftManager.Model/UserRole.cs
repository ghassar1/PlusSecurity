using AcSys.Core.Data.Identity.Model;

namespace AcSys.ShiftManager.Model
{
    public class UserRole : AcSysUserRole
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
