using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.DTOs
{
    public sealed record ProductDto(
        Guid Id,
        string Name,
        string? Description,
        string Sku,
        decimal BasePrice,
        int StockQuantity,
        bool IsActive,
        bool IsFeatured,
        Guid CategoryId,
        string CategoryName,
        Guid BrandId,
        string BrandName,
        IReadOnlyList<ProductImageDto> Images,
        IReadOnlyList<ProductAttributeDto> Attributes,
        IReadOnlyList<ProductVariantDto> Variants);
}
