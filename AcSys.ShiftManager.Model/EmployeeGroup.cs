using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcSys.Core.Data.Model.Base;
using AcSys.Core.Extensions;

namespace AcSys.ShiftManager.Model
{
    public class EmployeeGroup : NamedEntityBase
    {
        ICollection<User> employees = new Collection<User>();
        public virtual ICollection<User> Employees
        {
            get { return employees; }
            set { employees = value; }
        }

        public override bool HasChildRecords()
        {
            return Employees.Count > 0;
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Name);
        }

        public override string ToDescription()
        {
            return ToString();
        }
    }
}
