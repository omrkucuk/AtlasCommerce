using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasCommerce.BuildingBlocks.Common.Pagination
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalPage => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPage;
        public bool HasPreviousPage => Page > 1;

        public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
            => new() { Items = items, TotalCount = totalCount, Page = page, PageSize = pageSize };
    }
}
