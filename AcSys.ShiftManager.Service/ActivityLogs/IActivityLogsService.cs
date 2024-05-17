using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AcSys.ShiftManager.Data.ActivityLogs;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Service.Base;
using AcSys.ShiftManager.Service.Results;

namespace AcSys.ShiftManager.Service.ActivityLogs
{
    public interface IActivityLogsService : IApplicationService
    {
        Task<IListResult<ActivityLog, ActivityLogListItemDto>> Get(FindLogsQuery query);

        Task<ActivityLogDetailsDto> Get(Guid id);

        Task Delete(Guid id);

        Task Delete(List<Guid> ids);
    }
}
