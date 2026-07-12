using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Features.Brands.DTOs;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Brands.Queries.GetBrands
{
    public sealed class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, Result<IReadOnlyList<BrandDto>>>
    {
        private readonly ICatalogDbContext _context;

        public GetBrandsQueryHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IReadOnlyList<BrandDto>>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Brands.AsQueryable();

            if (request.IsActive.HasValue)
                query = query.Where(b => b.IsActive == request.IsActive.Value);

            var brands = await query
                .OrderBy(b => b.Name)
                .Select(b => new BrandDto(b.Id, b.Name, b.Description, b.LogoUrl, b.IsActive))
                .ToListAsync(cancellationToken);

            return Result.Success<IReadOnlyList<BrandDto>>(brands);
        }
    }
}
