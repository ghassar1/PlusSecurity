using System.Collections.Generic;
using AcSys.Core.Data.Querying;

namespace AcSys.ShiftManager.Service.Results
{
    public interface IListResult<TEntity, TDto> : IListResult<TDto>
    {
        ISearchQuery<TEntity> Query { get; set; }
    }

    public interface IListResult<TDto> : IServiceResult
    {
        int PageNo { get; set; }
        int PageSize { get; set; }

        int TotalItems { get; set; }

        int FilteredItems { get; set; }

        int TotalPages { get; set; }

        List<TDto> Items { get; set; }
    }
}
