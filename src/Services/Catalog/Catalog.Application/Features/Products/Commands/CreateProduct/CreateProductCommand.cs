using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.CreateProduct
{
    public sealed record CreateProductCommand(
        string Name,
        string? Description,
        string Sku,
        decimal BasePrice,
        int StockQuantity,
        Guid CategoryId,
        Guid BrandId) : IRequest<Result<Guid>>;
}
