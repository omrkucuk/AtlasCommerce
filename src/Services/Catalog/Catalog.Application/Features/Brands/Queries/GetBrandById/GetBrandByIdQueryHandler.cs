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

namespace Catalog.Application.Features.Brands.Queries.GetBrandById
{
    public sealed class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Result<BrandDto>>
    {
        private readonly ICatalogDbContext _context;

        public GetBrandByIdQueryHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<BrandDto>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            var brand = await _context.Brands
                .Where(b => b.Id == request.Id)
                .Select(b => new BrandDto(b.Id, b.Name, b.Description, b.LogoUrl, b.IsActive))
                .FirstOrDefaultAsync(cancellationToken);

            if (brand is null)
                return Result.Failure<BrandDto>(Error.NotFound("Brand.NotFound", "Marka bulunamadı."));

            return Result.Success(brand);
        }
    }
}
