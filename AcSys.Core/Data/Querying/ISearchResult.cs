namespace AcSys.Core.Data.Querying
{
    public interface ISearchResult<T>: IListResult<T>
    {
        ISearchQuery<T> Query { get; set; }

        int PageNo { get; set; }
        int PageSize { get; set; }

        int TotalRecords { get; set; }

        int FilteredRecords { get; set; }
    }
}
