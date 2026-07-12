using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.DTOs
{
    public sealed record ProductListDto(
        Guid Id,
        string Name,
        string Sku,
        decimal BasePrice,
        int StockQuantity,
        bool IsActive,
        bool IsFeatured,
        string CategoryName,
        string BrandName,
        string? MainImageUrl);
}
