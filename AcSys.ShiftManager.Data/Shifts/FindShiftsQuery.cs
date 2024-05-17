using System;
using System.Collections.Generic;
using System.Data.Entity;
using AcSys.Core.Data.Querying;
using AcSys.Core.Data.Specifications;
using AcSys.Core.Extensions;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.Shifts
{
    public class FindShiftsQuery : SearchQuery<Shift>
    {
        //public Guid? UserId { get; set; }

        public bool FilterUnGrouped { get; set; }
        public Guid? GroupId { get; set; }
        public bool IncludeOpenShifts { get; set; }

        public bool FilterOpenShiftsOnly { get; set; }


        //public Guid? EmployeeId { get; set; }
        public List<Guid> IncludeEmployeeIds { get; set; }
        public List<Guid> ExcludeEmployeeIds { get; set; }

        public List<Guid> IncludeShiftIds { get; set; }
        public List<Guid> ExcludeShiftIds { get; set; }

        public bool CompareExactDateTime { get; set; }
        public bool FindOverlappingShifts { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        //public string Start { get; set; }
        //public string End { get; set; }

        public FindShiftsQuery()
        {
            PageSize = -1;
            PageNo = -1;

            SortColumn = "StartTime";
            SortType = SortType.Descending;
            
            IncludeOpenShifts = true;
            FilterOpenShiftsOnly = false;

            FilterUnGrouped = false;

            IncludeEmployeeIds = new List<Guid>();
            ExcludeEmployeeIds = new List<Guid>();

            IncludeShiftIds = new List<Guid>();
            ExcludeShiftIds = new List<Guid>();
        }

        public override ISpecification<Shift> ToSpec()
        {
            if (!StartDate.HasValue) StartDate = DateTime.Today.BeginningOfTheWeek();
            if (!EndDate.HasValue) EndDate = DateTime.Today.EndOfTheWeek();

            ISpecification<Shift> spec = new Specification<Shift>();

            if (FilterUnGrouped)
            {
                if (IncludeOpenShifts)
                {
                    spec = spec.And(o => o.Employee == null || o.Employee.EmployeeGroup == null);
                }
                else
                {
                    spec = spec.And(o => o.Employee != null && o.Employee.EmployeeGroup == null);
                }
            }
            else if (GroupId.IsNotNullOrEmpty())
            {
                if (IncludeOpenShifts)
                {
                    spec = spec.And(o => o.Employee == null 
                        || (o.Employee.EmployeeGroup != null && o.Employee.EmployeeGroup.Id == GroupId));
                }
                else
                {
                    spec = spec.And(o => o.Employee != null && o.Employee.EmployeeGroup != null && o.Employee.EmployeeGroup.Id == GroupId);
                }
            }

            //if (!FilterOpen)
            //{
            //    //spec = spec.And(o => !o.IsOpen);
            //    spec = spec.And(o => o.Employee != null);
            //}

            if (FilterOpenShiftsOnly)
            {
                spec = spec.And(o => o.Employee == null && o.IsOpen);
            }
            else
            {
                //if (UserId.IsNotNullOrEmpty())
                //{
                //    spec = spec.And(o => o.Employee.Id == UserId);
                //}

                if (IncludeEmployeeIds != null && IncludeEmployeeIds.Count > 0)
                {
                    spec = spec.And(o => o.Employee != null && IncludeEmployeeIds.Contains(o.Employee.Id));
                }

                if (ExcludeEmployeeIds != null && ExcludeEmployeeIds.Count > 0)
                {
                    spec = spec.And(o => o.Employee != null && !ExcludeEmployeeIds.Contains(o.Employee.Id));
                }
            }

            if (IncludeShiftIds != null && IncludeShiftIds.Count > 0)
            {
                spec = spec.And(o => IncludeShiftIds.Contains(o.Id));
            }

            if (ExcludeShiftIds != null && ExcludeShiftIds.Count > 0)
            {
                spec = spec.And(o => !ExcludeShiftIds.Contains(o.Id));
            }

            if (SearchCriteria.IsNotNullOrWhiteSpace())
            {
                string criteria = SearchCriteria.ToUpper();
                spec = spec.And(o => o.Title.ToUpper().Contains(criteria) || o.Notes.ToUpper().Contains(criteria));
            }

            if (FindOverlappingShifts && StartDate.HasValue && EndDate.HasValue)
            {
                // truncate to minutes
                StartDate = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day, StartDate.Value.Hour, StartDate.Value.Minute, 0, 0);
                EndDate = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day, EndDate.Value.Hour, EndDate.Value.Minute, 0, 0);

                //bool overlap = startTime < shift.EndTime && shift.StartTime < endTime;
                // Use <= instead of < if you want the shifts that just touch each other on first or last minute.)
                spec = spec.And(o => StartDate < o.EndTime && o.StartTime < EndDate);
                //spec = spec.And(o => Start <= o.EndTime && o.StartTime <= End);
            }
            else
            {
                if (StartDate.HasValue)
                {
                    if (CompareExactDateTime)
                        spec = spec.And(o => o.StartTime >= StartDate.Value);
                    else
                    {
                        StartDate = StartDate.Value.Date;
                        spec = spec.And(o => DbFunctions.TruncateTime(o.StartTime) >= StartDate.Value);
                    }

                }

                if (EndDate.HasValue)
                {
                    if (CompareExactDateTime)
                        spec = spec.And(o => o.StartTime <= EndDate.Value);
                    else
                    {
                        EndDate = EndDate.Value.Date;
                        spec = spec.And(o => DbFunctions.TruncateTime(o.StartTime) <= EndDate.Value);
                    }
                }
            }

            spec = spec.OrderByDescending(o => o.StartTime);

            spec.IncludePath = "Employee";

            return spec;
        }
    }
}
