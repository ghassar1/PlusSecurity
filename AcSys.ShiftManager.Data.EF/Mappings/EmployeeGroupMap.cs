using AcSys.Core.Data.EF.Mappings.Base;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Mappings
{
    public class EmployeeGroupMap : NamedEntityMapBase<EmployeeGroup>
    {
        public EmployeeGroupMap()
        {
            //HasMany(e => e.Employees)
            //    .WithMany(e => e.Groups);

            HasMany(e => e.Employees)
                .WithOptional(e => e.EmployeeGroup);
        }
    }
}
