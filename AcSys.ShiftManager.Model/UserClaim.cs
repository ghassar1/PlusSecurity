using AcSys.Core.Data.Identity.Model;

namespace AcSys.ShiftManager.Model
{
    public class UserClaim : AcSysUserClaim
    {
        public virtual User User { get; set; }
    }
}
