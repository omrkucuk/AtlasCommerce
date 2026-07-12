using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Brands.Commands.DeleteBrand
{
    public sealed record DeleteBrandCommand(Guid Id): IRequest<Result>;
}
