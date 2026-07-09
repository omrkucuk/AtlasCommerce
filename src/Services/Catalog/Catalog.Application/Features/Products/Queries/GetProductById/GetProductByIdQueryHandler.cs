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

namespace Catalog.Application.Features.Products.Queries.GetProductById
{
    public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly ICatalogDbContext _context;

        public GetProductByIdQueryHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.Attributes)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product is null)
                return Result.Failure<ProductDto>(Error.NotFound("Product.NotFound", "Ürün bulunamadı."));

            var dto = new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Sku,
                product.BasePrice,
                product.StockQuantity,
                product.IsActive,
                product.IsFeatured,
                product.CategoryId,
                product.Category?.Name ?? string.Empty,
                product.BrandId,
                product.Brand?.Name ?? string.Empty,
                product.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.DisplayOrder)
                    .Select(i => new ProductImageDto(i.Id, i.ImageUrl, i.IsMain, i.DisplayOrder))
                    .ToList(),
                product.Attributes
                    .Select(a => new ProductAttributeDto(a.Id, a.Name, a.Value))
                    .ToList(),
                product.Variants
                    .Select(v => new ProductVariantDto(
                        v.Id,
                        v.Sku,
                        v.PriceOverride,
                        v.StockQuantity,
                        v.IsActive,
                        v.Attributes
                            .Select(a => new VariantAttributeDto(a.Id, a.Name, a.Value))
                            .ToList()))
                    .ToList());


            return Result.Success(dto);
        }
    }
}
