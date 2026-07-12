using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Brands.Commands.DeleteBrand
{
    public sealed class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, Result>
    {
        private readonly ICatalogDbContext _context;

        public DeleteBrandCommandHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _context.Brands
                .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (brand is null)
                return Result.Failure(Error.NotFound("Brand.NotFound", "Marka bulunamadı."));

            var hasProducts = await _context.Products
                .AnyAsync(p => p.BrandId == request.Id, cancellationToken);

            if (hasProducts)
                return Result.Failure(Error.Conflict("Brand.HasProducts", "Ürünleri olan bir marka silinemez."));

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
