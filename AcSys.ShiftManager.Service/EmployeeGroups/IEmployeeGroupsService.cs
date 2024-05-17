using AcSys.ShiftManager.Service.Base;
using AcSys.ShiftManager.Service.Results;
using System;
using System.Threading.Tasks;

namespace AcSys.ShiftManager.Service.EmployeeGroups
{
    public interface IEmployeeGroupsService : IApplicationService
    {
        Task<IListResult<EmployeeGroupDto>> Get();

        Task<EmployeeGroupDto> Get(Guid id);

        Task Update(Guid id, EmployeeGroupDto dto);

        Task<Guid> Create(EmployeeGroupDto dto);

        Task Delete(Guid id);
    }
}
