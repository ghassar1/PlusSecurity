using System;
using System.Data.Entity;
using AcSys.Core.Data.Querying;
using AcSys.Core.Data.Specifications;
using AcSys.Core.Extensions;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Model.Helpers;

namespace AcSys.ShiftManager.Data.Shifts
{
    public class AttendanceReportShiftsQuery : SearchQuery<Shift>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Guid? EmployeeId { get; set; }

        public Enums.ShiftStatus? ShiftStatus { get; set; }
        public Enums.ShiftStartStatus? StartStatus { get; set; }
        public Enums.ShiftEndStatus? EndStatus { get; set; }

        public AttendanceReportShiftsQuery()
        {
            PageSize = -1;
            PageNo = -1;

            SortColumn = "StartTime";
            SortType = SortType.Descending;
        }

        public AttendanceReportShiftsQuery(DateTime startDate, DateTime endDate, Guid? employeeId = null, Enums.ShiftStatus? status = null, Enums.ShiftStartStatus? startStatus = null, Enums.ShiftEndStatus? endStatus = null) 
            : this()
        {
            StartDate = startDate;
            EndDate = endDate;
            EmployeeId = employeeId;

            ShiftStatus = status;
            StartStatus = startStatus;
            EndStatus = endStatus;
        }

        public override ISpecification<Shift> ToSpec()
        {
            if (!StartDate.HasValue) StartDate = DateTime.Today.BeginningOfTheWeek();
            if (!EndDate.HasValue) EndDate = DateTime.Today.EndOfTheWeek();

            ISpecification<Shift> spec = new Specification<Shift>(o => o.Employee != null);

            if (EmployeeId.HasValue)
            {
                spec = spec.And(o => o.Employee.Id == EmployeeId.Value);
            }

            if (StartDate.HasValue)
            {
                StartDate = StartDate.Value.Date;
                spec = spec.And(o => DbFunctions.TruncateTime(o.StartTime) >= StartDate.Value);
            }

            if (EndDate.HasValue)
            {
                EndDate = EndDate.Value.Date;
                spec = spec.And(o => DbFunctions.TruncateTime(o.StartTime) <= EndDate.Value);
            }

            if (ShiftStatus.HasValue)
            {
                spec = spec.And(o => o.Status == ShiftStatus.Value);
            }

            if (StartStatus.HasValue)
            {
                spec = spec.And(o => o.StartStatus == StartStatus.Value);
            }

            if (EndStatus.HasValue)
            {
                spec = spec.And(o => o.EndStatus == EndStatus.Value);
            }

            spec = spec.OrderBy(o => new { o.Employee.FirstName, o.Employee.LastName, o.StartTime });
            //spec = spec.OrderByDescending(o => o.StartTime);
            //spec = spec.OrderByDescending(o => o.Employee);

            spec.IncludePath = "Employee";

            return spec;
        }
    }
}
