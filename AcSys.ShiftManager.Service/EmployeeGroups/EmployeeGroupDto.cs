using AcSys.ShiftManager.Service.Common;

namespace AcSys.ShiftManager.Service.EmployeeGroups
{
    public class EmployeeGroupDto : NamedEntityDto
    {
        public EmployeeGroupDto()
        {
            NoOfEmployees = 0;
        }

        public int NoOfEmployees { get; set; }
    }
}
