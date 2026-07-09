using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.DTOs
{
    public sealed record ProductVariantDto(
        Guid Id,
        string Sku,
        decimal? PriceOverride,
        int StockQuantity,
        bool IsActive,
        IReadOnlyList<VariantAttributeDto> Attributes);
}
