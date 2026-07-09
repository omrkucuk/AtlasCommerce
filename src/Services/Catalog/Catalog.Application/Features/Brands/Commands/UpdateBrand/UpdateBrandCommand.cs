using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Brands.Commands.UpdateBrand
{
    public sealed record UpdateBrandCommand(
        Guid Id,
        string Name,
        string? Description,
        bool IsActive) : IRequest<Result>;
}
