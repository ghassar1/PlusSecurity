using AcSys.Core.Data.Identity.Model;

namespace AcSys.ShiftManager.Model
{
    public class UserLogin : AcSysUserLogin
    {
        public virtual User User { get; set; }
    }
}
