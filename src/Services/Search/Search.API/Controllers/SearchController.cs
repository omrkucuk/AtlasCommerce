using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Search.API.Models;
using Search.API.Services;

namespace Search.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ElasticsearchService _elasticsearchService;

        public SearchController(ElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }

        [HttpGet("products")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string? q,
            [FromQuery] Guid? categoryId,
            [FromQuery] Guid? brandId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? isActive,
            [FromQuery] bool? isFeatured,
            [FromQuery] bool? inStock,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            // PageSize sınırla
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var request = new ProductSearchRequest
            {
                Query = q,
                CategoryId = categoryId,
                BrandId = brandId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                IsActive = isActive ?? true,
                IsFeatured = isFeatured,
                InStock = inStock,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Page = page,
                PageSize = pageSize
            };

            var result = await _elasticsearchService.SearchProductsAsync(request, cancellationToken);

            return Ok(new
            {
                items = result.Items,
                totalCount = result.TotalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize),
                hasNextPage = page * pageSize < result.TotalCount,
                hasPreviousPage = page > 1
            });
        }

        [HttpGet("products/autocomplete")]
        public async Task<IActionResult> Autocomplete([FromQuery] string prefix, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(prefix) || prefix.Length < 2)
                return Ok(Array.Empty<string>());

            var suggestions = await _elasticsearchService.AutocompleteAsync(prefix, cancellationToken);

            return Ok(suggestions);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> SearchCategories(
            [FromQuery] string? q,
            [FromQuery] bool? isActive,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _elasticsearchService.SearchCategoryAsync(q, isActive, page, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpGet("brands")]
        public async Task<IActionResult> SearchBrands(
            [FromQuery] string? q,
            [FromQuery] bool? isActive,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _elasticsearchService.SearchBrandsAsync(q, isActive, page, pageSize, cancellationToken);
            return Ok(result);
        }
    }
}
