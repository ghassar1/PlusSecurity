using System.Collections.Generic;

namespace AcSys.Core.Data.Querying
{
    public class SearchResults<T> : ISearchResult<T>
    {
        public SearchResults()
        {
            Records = new List<T>();
        }

        public ISearchQuery<T> Query { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int FilteredRecords { get; set; }

        public List<T> Records { get; set; }
    }
}
