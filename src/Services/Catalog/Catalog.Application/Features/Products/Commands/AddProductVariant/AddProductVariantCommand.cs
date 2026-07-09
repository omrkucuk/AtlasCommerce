using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.AddProductVariant
{
    public sealed record VariantAttributeRequest(string Name, string Value);
    public sealed record AddProductVariantCommand(
        Guid ProductId,
        string Sku,
        decimal? PriceOverride,
        int StockQuantity,
        List<VariantAttributeRequest> Attributes) : IRequest<Result<Guid>>;
}
