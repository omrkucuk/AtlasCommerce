using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.UpdateProduct
{
    public sealed record UpdateProductCommand(
        Guid Id,
        string Name,
        string? Description,
        decimal BasePrice,
        int StockQuantity,
        Guid CategoryId,
        Guid BrandId,
        bool IsActive,
        bool IsFeatured) : IRequest<Result>;
}
