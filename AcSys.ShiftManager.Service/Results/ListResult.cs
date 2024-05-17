using System;
using System.Collections.Generic;
using AcSys.Core.Data.Querying;
using AcSys.Core.ObjectMapping;

namespace AcSys.ShiftManager.Service.Results
{
    public class ListResult<TEntity, TDto> : ListResult<TDto>, IListResult<TEntity, TDto>
    {
        Action<TEntity, TDto> ListAction { get; set; }
        ISearchResult<TEntity> SearchResults { get; set; }

        public ISearchQuery<TEntity> Query { get; set; }

        public ListResult()
            : base()
        {
        }

        public ListResult(List<TDto> items)
            : base(items)
        {
            Items = items;
        }

        public ListResult(ISearchQuery<TEntity> query)
            : this()
        {
            //Paging = query.Paging;
            //Sort = query.Sort;
            Query = query;
        }

        public ListResult(ISearchQuery<TEntity> query, ISearchResult<TEntity> results, Action<TEntity, TDto> listAction = null)
            : this(query)
        {
            PageSize = query.PageSize;
            PageNo = query.PageNo;

            TotalItems = results.TotalRecords;
            FilteredItems = results.FilteredRecords;

            if (query.PageSize > 0)
                TotalPages = (int)Math.Ceiling((decimal)TotalItems / query.PageSize);

            ListAction = listAction;

            if (ListAction == null)
            {
                Items = ObjectMapper.Map<List<TEntity>, List<TDto>>(results.Records);
            }
            else
            {
                foreach (TEntity e in results.Records)
                {
                    TDto dto = ObjectMapper.Map<TEntity, TDto>(e);
                    ListAction(e, dto);
                    Items.Add(dto);
                }
            }
        }
    }

    public class ListResult<TDto> : ServiceResult, IListResult<TDto>
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public int TotalItems { get; set; }
        public int FilteredItems { get; set; }
        public int TotalPages { get; set; }

        public List<TDto> Items { get; set; }

        public ListResult()
        {
            TotalPages = 0;
            Items = new List<TDto>();
        }

        public ListResult(List<TDto> items)
            : this()
        {
            Items = items;
        }
    }
}
