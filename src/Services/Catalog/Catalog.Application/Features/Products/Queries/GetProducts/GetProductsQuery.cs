using AtlasCommerce.BuildingBlocks.Common.Pagination;
using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Features.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Queries.GetProducts
{
    public sealed record GetProductsQuery(
        int Page = 1,
        int PageSize = 20,
        string? Search = null,
        Guid? CategoryId = null,
        Guid? BrandId = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        bool? IsActive = null,
        bool? IsFeatured = null,
        string? SortBy = null,
        string? SortOrder = null) : IRequest<Result<PagedResult<ProductListDto>>>;
}
