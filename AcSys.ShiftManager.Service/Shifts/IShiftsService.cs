using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AcSys.ShiftManager.Data.Shifts;
using AcSys.ShiftManager.Service.Base;

namespace AcSys.ShiftManager.Service.Shifts
{
    public interface IShiftsService : IApplicationService
    {
        Task<RotaDto> Get(FindShiftsQuery query);

        Task<ShiftDto> Get(Guid id);

        Task<List<Guid>> Create(CreateShiftDto dto);

        Task Update(Guid id, UpdateShiftDto dto);

        Task Assign(Guid id, AssignShiftDto dto);

        Task Take(Guid id);
        Task Leave(Guid id);
        Task ClockIn(Guid id);
        Task ClockOut(Guid id);

        Task Delete(Guid id);

        Task Delete(List<Guid> ids);

        Task<List<AttendanceReportRowDto>> GetAttendanceData(AttendanceReportShiftsQuery query);

        Task<AttendanceSummaryReportDto> GetAttendanceSummaryData(AttendanceReportShiftsQuery query);

        Task<DashboardDto> GetDashboardData();
    }
}
