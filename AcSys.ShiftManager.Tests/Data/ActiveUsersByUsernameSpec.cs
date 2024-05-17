using AcSys.Core.Data.Specifications;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Tests.Data
{
    public class ActiveUsersByUsernameSpec : SpecificationBase<User>
    {
        public ActiveUsersByUsernameSpec(string username)
        {
            Predicate = p => p.UserName.ToUpper() == username.ToUpper() && p.IsActive();

            FetchStrategy = new FetchStrategyBase<User>();//.Include(p => p.Roles);
        }
    }
}
