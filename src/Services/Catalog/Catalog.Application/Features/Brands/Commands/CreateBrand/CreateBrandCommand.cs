using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Brands.Commands.CreateBrand
{
    public sealed record CreateBrandCommand(
        string Name,
        string? Description): IRequest<Result<Guid>>;
}
