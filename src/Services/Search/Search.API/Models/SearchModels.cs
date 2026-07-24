namespace Search.API.Models
{

    public sealed class ProductSearchRequest
    {
        public string? Query { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? InStock { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public sealed class SearchResult<T>
    {
        public IReadOnlyList<T> Items { get; init; }
        public long TotalCount { get; init; }

        public SearchResult(IReadOnlyList<T> items, long totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
    }
}
