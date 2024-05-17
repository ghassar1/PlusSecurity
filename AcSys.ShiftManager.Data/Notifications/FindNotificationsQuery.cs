using System;
using System.Data.Entity.SqlServer;
using System.Linq.Expressions;
using AcSys.Core.Data.Querying;
using AcSys.Core.Data.Specifications;
using AcSys.Core.Extensions;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.Notifications
{
    public class FindNotificationsQuery : SearchQuery<Notification>
    {
        public FindNotificationsQuery()
        {
            PageNo = 1;
            PageSize = 10;

            SortColumn = "Date";
            SortType = SortType.Descending;
        }

        public override ISpecification<Notification> ToSpec()
        {
            ISpecification<Notification> spec = new Specification<Notification>();
            
            if (SearchCriteria.IsNotNullOrWhiteSpace())
            {
                ISpecification<Notification> criteriaSpec = new Specification<Notification>(
                    o => (o.Sender.FirstName + " " + o.Sender.LastName + " " + o.Sender.Email + " " + o.Title + " " + o.Text + " " +
                    (SqlFunctions.DateName("day", o.SentAt) + " " + SqlFunctions.DateName("month", o.SentAt) + " " + SqlFunctions.DateName("year", o.SentAt)))
                    .Contains(SearchCriteria.ToUpper()));

                spec = spec.And(criteriaSpec);
            }

            //if (SortColumn.IsNotNullOrWhiteSpace()) { }
            //Expression<Func<Notification, string>> stringDateSortExp = o => (SqlFunctions.DateName("year", o.SentAt) + "-" + SqlFunctions.DateName("month", o.SentAt) + "-" + SqlFunctions.DateName("day", o.SentAt) + "-" + SqlFunctions.DateName("hour", o.SentAt) + "-" + SqlFunctions.DateName("minute", o.SentAt) + "-" + SqlFunctions.DateName("second", o.SentAt) + "-" + SqlFunctions.DateName("millisecond", o.SentAt));
            Expression<Func<Notification, string>> toSortExp = o => "{0} {1}".FormatWith(o.Sender.FirstName, o.Sender.LastName);
            Expression<Func<Notification, string>> titleSortExp = o => o.Title;
            Expression<Func<Notification, DateTime>> dateSortExp = o => o.SentAt;

            switch (SortColumn.ToUpper())
            {
                case "TO":
                    spec = SortType == SortType.Ascending ? spec.OrderBy(toSortExp) : spec.OrderByDescending(toSortExp);
                    break;

                case "TITLE":
                    spec = SortType == SortType.Ascending ? spec.OrderBy(titleSortExp) : spec.OrderByDescending(titleSortExp);
                    break;

                case "DATE":
                    //spec = SortType == SortType.Ascending ? spec.OrderBy(stringDateSortExp) : spec.OrderByDescending(stringDateSortExp);
                    spec = SortType == SortType.Ascending ? spec.OrderBy(dateSortExp) : spec.OrderByDescending(dateSortExp);
                    break;

                default:
                    //spec = SortType == SortType.Ascending ? spec.OrderBy(stringDateSortExp) : spec.OrderByDescending(stringDateSortExp);
                    spec = SortType == SortType.Ascending ? spec.OrderBy(dateSortExp) : spec.OrderByDescending(dateSortExp);
                    break;
            }

            if (PageSize > 0)
            {
                spec = spec.Skip((PageNo - 1) * PageSize).Take(PageSize);
            }

            return spec;
        }
    }
}
