using System;
using System.Data.Entity;
using AcSys.Core.Data.Querying;
using AcSys.Core.Data.Specifications;
using AcSys.Core.Extensions;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.ActivityLogs
{
    public class FindLogsQuery : SearchQuery<ActivityLog>
    {
        public Guid? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        //public string StartDate { get; set; }
        //public string EndDate { get; set; }

        public FindLogsQuery()
        {
            SortColumn = "Date";
            SortType = SortType.Descending;
        }

        public override ISpecification<ActivityLog> ToSpec()
        {
            ISpecification<ActivityLog> spec = new Specification<ActivityLog>();

            if (UserId.IsNotNullOrEmpty())
            {
                spec = spec.And(o => o.User.Id == UserId);
            }

            if (SearchCriteria.IsNotNullOrWhiteSpace())
            {
                spec = spec.And(o => o.Description.ToUpper().Contains(SearchCriteria.ToUpper()));
            }

            //ISpecification<ActivityLog> dateRangeSpec = new Specification<ActivityLog>(
            //    o => (SqlFunctions.DateName("day", o.DateTimeStamp) + " " + SqlFunctions.DateName("month", o.DateTimeStamp) + " " + SqlFunctions.DateName("year", o.DateTimeStamp))
            //    .Contains(SearchCriteria.ToUpper()));

            if (StartDate.HasValue)
            {
                StartDate = StartDate.Value.Date;
                spec = spec.And(o => DbFunctions.TruncateTime(o.DateTimeStamp.Value) >= StartDate.Value);
            }

            if (EndDate.HasValue)
            {
                EndDate = EndDate.Value.Date;
                spec = spec.And(o => DbFunctions.TruncateTime(o.DateTimeStamp.Value) <= EndDate.Value);
            }

            //DateTime startDate;
            //if (StartDate.IsNotNullOrWhiteSpace() && DateTime.TryParse(StartDate, out startDate))
            //{
            //    spec = spec.And(o => o.DateTimeStamp.Value.Date >= startDate.Date);
            //}

            //DateTime endDate;
            //if (EndDate.IsNotNullOrWhiteSpace() && DateTime.TryParse(EndDate, out endDate))
            //{
            //    spec = spec.And(o => o.DateTimeStamp.Value.Date <= endDate.Date);
            //}

            spec = spec.OrderByDescending(o => o.DateTimeStamp.Value);
            //spec = spec.OrderByDescending(o => o.User.FirstName);

            if (PageSize > 0)
            {
                spec = spec.Skip((PageNo - 1) * PageSize).Take(PageSize);
            }

            return spec;
        }
    }
}
