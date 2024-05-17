using System;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using AcSys.Core.Data.Querying;
using AcSys.Core.Data.Specifications;
using AcSys.Core.Extensions;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.Messages
{
    public class FindMyInBoxMessagesQuery : SearchQuery<Message>
    {
        public Guid UserId { get; set; }

        public FindMyInBoxMessagesQuery()
        {
            SortColumn = "DATE";
            SortType = SortType.Descending;
        }

        public ISpecification<Message> ToSpec(Guid userId)
        {
            UserId = userId;
            return ToSpec();
        }

        public override ISpecification<Message> ToSpec()
        {
            if (UserId == Guid.Empty)
                throw new ArgumentNullException(nameof(UserId), "UserId must be specified.");

            ISpecification<Message> spec = new Specification<Message>(o => o.Recipients.Any(r => r.Id == UserId));
            
            if (SearchCriteria.IsNotNullOrWhiteSpace())
            {
                ISpecification<Message> criteriaSpec = new Specification<Message>(o =>
                (o.Sender.FirstName + " " + o.Sender.LastName + " " + o.Sender.Email + " " +
                o.Subject + " " + o.Text + " " +
                (SqlFunctions.DateName("day", o.SentAt) + " " + 
                SqlFunctions.DateName("month", o.SentAt) + " " + 
                SqlFunctions.DateName("year", o.SentAt)))
                .Contains(SearchCriteria.ToUpper()));


                //criteriaSpec = criteriaSpec.Or(o => o.Text.ToUpper().Contains(SearchCriteria.ToUpper()));

                //criteriaSpec = criteriaSpec.Or(o => o.SentAt.ToString("dd MMMM yyyy HH:mm a").ToUpper().Contains(SearchCriteria.ToUpper()));
                //criteriaSpec = criteriaSpec.Or(o => (SqlFunctions.DateName("day", o.SentAt) + " " + SqlFunctions.DateName("month", o.SentAt) + " " + SqlFunctions.DateName("year", o.SentAt)).Contains(SearchCriteria.ToUpper()));

                spec = spec.And(criteriaSpec);
            }

            //if (SortColumn.IsNotNullOrWhiteSpace()) { }
            //Expression<Func<Message, string>> stringDateSortExp = o => (SqlFunctions.DateName("year", o.SentAt) + "-" + SqlFunctions.DateName("month", o.SentAt) + "-" + SqlFunctions.DateName("day", o.SentAt) + "-" + SqlFunctions.DateName("hour", o.SentAt) + "-" + SqlFunctions.DateName("minute", o.SentAt) + "-" + SqlFunctions.DateName("second", o.SentAt) + "-" + SqlFunctions.DateName("millisecond", o.SentAt));
            Expression<Func<Message, string>> toSortExp = o => "{0} {1}".FormatWith(o.Sender.FirstName, o.Sender.LastName);
            Expression<Func<Message, string>> subjectSortExp = o => o.Subject;
            Expression<Func<Message, DateTime>> dateSortExp = o => o.SentAt;

            switch (SortColumn.ToUpper())
            {
                case "TO":
                    spec = SortType == SortType.Ascending ? spec.OrderBy(toSortExp) : spec.OrderByDescending(toSortExp);
                    break;

                case "SUBJECT":
                    spec = SortType == SortType.Ascending ? spec.OrderBy(subjectSortExp) : spec.OrderByDescending(subjectSortExp);
                    break;

                case "DATE":
                    spec = SortType == SortType.Ascending ? spec.OrderBy(dateSortExp) : spec.OrderByDescending(dateSortExp);
                    break;

                default:
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
