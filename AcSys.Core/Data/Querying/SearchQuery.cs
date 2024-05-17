using AcSys.Core.Data.Model.Base;
using AcSys.Core.Data.Specifications;

namespace AcSys.Core.Data.Querying
{
    public class SearchQuery<T> : ISearchQuery<T>
    {
        public SearchQuery()
        {
            Status = null;

            SearchCriteria = string.Empty;

            PageNo = 1;
            PageSize = 10;

            SortColumn = string.Empty;
            SortType = SortType.Ascending;
        }

        public EntityStatus? Status { get; set; }

        public string SearchCriteria { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public string SortColumn { get; set; }
        public SortType SortType { get; set; }

        public virtual ISpecification<T> ToSpec()
        {
            return new Specification<T>();
        }
    }
}
