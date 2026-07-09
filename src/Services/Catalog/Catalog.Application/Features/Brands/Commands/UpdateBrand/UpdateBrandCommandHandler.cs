using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Brands.Commands.UpdateBrand
{
    public sealed class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Result>
    {
        private readonly ICatalogDbContext _context;

        public UpdateBrandCommandHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _context.Brands
                .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (brand is null)
                return Result.Failure(Error.NotFound("Brand.NotFound", "Marka bulunamadı."));

            var nameConflict = await _context.Brands
                .AnyAsync(b => b.Name == request.Name && b.Id != request.Id, cancellationToken);

            if (nameConflict)
                return Result.Failure(Error.Conflict("Brand.NameExists", "Bu isimde başka bir marka zaten var."));

            brand.Update(request.Name, request.Description, request.IsActive);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
