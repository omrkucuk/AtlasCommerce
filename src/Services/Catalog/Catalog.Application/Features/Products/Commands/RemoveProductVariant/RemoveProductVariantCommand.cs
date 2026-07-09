using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.RemoveProductVariant
{
    public sealed record RemoveProductVariantCommand(
        Guid ProductId,
        Guid VariantId): IRequest<Result>;
}
