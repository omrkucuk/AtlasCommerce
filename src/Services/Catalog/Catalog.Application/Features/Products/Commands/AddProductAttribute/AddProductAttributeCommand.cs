using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.AddProductAttribute
{
    public sealed record AddProductAttributeCommand(
        Guid ProductId,
        string Name,
        string Value) : IRequest<Result<Guid>>;
}
