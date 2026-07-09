using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.RemoveProductVariant
{
    public sealed class RemoveProductVariantCommandHandler : IRequestHandler<RemoveProductVariantCommand, Result>
    {
        private readonly ICatalogDbContext _context;

        public RemoveProductVariantCommandHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(RemoveProductVariantCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
            .Include(p => p.Variants)
                .ThenInclude(v => v.Attributes)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product is null)
                return Result.Failure(Error.NotFound("Product.NotFound", "Ürün bulunamadı."));

            try
            {
                product.RemoveVariant(request.VariantId);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(Error.NotFound("Product.VariantNotFound", ex.Message));
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
