using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Features.Brands.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Brands.Queries.GetBrands
{
    public sealed record GetBrandsQuery(bool? IsActive = null) : IRequest<Result<IReadOnlyList<BrandDto>>>;
}
