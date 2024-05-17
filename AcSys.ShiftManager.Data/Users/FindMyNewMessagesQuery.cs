using System;
using System.Linq;
using AcSys.Core.Data.Querying;
using AcSys.Core.Data.Specifications;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.Users
{
    public class FindMyNewMessagesQuery : SearchQuery<Message>
    {
        public User User { get; set; }

        public FindMyNewMessagesQuery()
        {
            PageSize = 0;

            SortColumn = "Date";
            SortType = SortType.Ascending;
        }

        public ISpecification<Message> ToSpec(User user)
        {
            User = user;
            return ToSpec();
        }

        public override ISpecification<Message> ToSpec()
        {
            if (User == null) throw new ArgumentNullException(nameof(User), "User must be specified.");

            ISpecification<Message> spec = new Specification<Message>(
                o => o.Recipients.Any(r => r.Id == User.Id) 
                && !o.Views.Any(u => u.User.Id == User.Id));

            spec = spec.OrderByDescending(o => o.SentAt);
            return spec;
        }
    }
}
