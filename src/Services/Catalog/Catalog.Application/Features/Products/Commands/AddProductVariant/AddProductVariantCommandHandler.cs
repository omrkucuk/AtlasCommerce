using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.AddProductVariant
{
    public sealed class AddProductVariantCommandHandler : IRequestHandler<AddProductVariantCommand, Result<Guid>>
    {
        private readonly ICatalogDbContext _context;

        public AddProductVariantCommandHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(AddProductVariantCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product is null)
                return Result.Failure<Guid>(Error.NotFound("Product.NotFound", "Ürün bulunamadı."));

            var attributeTuples = request.Attributes
                .Select(a => (a.Name, a.Value))
                .ToList();

            try
            {
                product.AddVariant(request.Sku, request.PriceOverride, request.StockQuantity, attributeTuples);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<Guid>(Error.Conflict("Product.VariantExists", ex.Message));
            }

            await _context.SaveChangesAsync(cancellationToken);

            var addedVariant = product.Variants
                .First(v => v.Sku == request.Sku);

            return Result.Success(addedVariant.Id);
        }
    }
}
