using AcSys.Core.Data.Model.Base;

namespace AcSys.Core.Data.Querying
{
    public interface ISearchQuery<T> : IQuerySpecificatoin<T>
    {
        EntityStatus? Status { get; set; }
        string SearchCriteria { get; set; }

        int PageNo { get; set; }
        int PageSize { get; set; }

        string SortColumn { get; set; }
        SortType SortType { get; set; }
    }
}
