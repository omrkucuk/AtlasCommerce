using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.RemoveProductAttribute
{
    public sealed record RemoveProductAttributeCommand(
        Guid ProductId,
        Guid AttributeId) : IRequest<Result>;
}
