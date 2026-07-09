using AtlasCommerce.BuildingBlocks.Common.Pagination;
using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Features.Products.DTOs;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Queries.GetProducts
{
    public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductListDto>>>
    {
        private readonly ICatalogDbContext _context;

        public GetProductsQueryHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<ProductListDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Products.AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(p =>
                    p.Name.Contains(request.Search) ||
                    p.Sku.Contains(request.Search));

            if (request.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);

            if (request.BrandId.HasValue)
                query = query.Where(p => p.BrandId == request.BrandId.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.BasePrice >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.BasePrice <= request.MaxPrice.Value);

            if (request.IsActive.HasValue)
                query = query.Where(p => p.IsActive == request.IsActive.Value);

            if (request.IsFeatured.HasValue)
                query = query.Where(p => p.IsFeatured == request.IsFeatured.Value);

            // Sorting
            query = (request.SortBy?.ToLower(), request.SortOrder?.ToLower()) switch
            {
                ("price", "desc") => query.OrderByDescending(p => p.BasePrice),
                ("price", _) => query.OrderBy(p => p.BasePrice),
                ("stock", "desc") => query.OrderByDescending(p => p.StockQuantity),
                ("stock", _) => query.OrderBy(p => p.StockQuantity),
                ("name", "desc") => query.OrderByDescending(p => p.Name),
                ("createdat", "desc") => query.OrderByDescending(p => p.CreatedAt),
                ("createdat", _) => query.OrderBy(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductListDto(
                    p.Id,
                    p.Name,
                    p.Sku,
                    p.BasePrice,
                    p.StockQuantity,
                    p.IsActive,
                    p.IsFeatured,
                    p.Category != null ? p.Category.Name : string.Empty,
                    p.Brand != null ? p.Brand.Name : string.Empty,
                    p.Images
                        .Where(i => i.IsMain)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()))
                .ToListAsync(cancellationToken);

            var result = PagedResult<ProductListDto>.Create(items, totalCount, request.Page, request.PageSize);

            return Result.Success(result);
        }
    }
}
