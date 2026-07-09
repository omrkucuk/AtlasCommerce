using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.RemoveProductAttribute
{
    public sealed class RemoveProductAttributeCommandHandler : IRequestHandler<RemoveProductAttributeCommand, Result>
    {
        private readonly ICatalogDbContext _context;

        public RemoveProductAttributeCommandHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(RemoveProductAttributeCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(p => p.Attributes)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product is null)
                return Result.Failure(Error.NotFound("Product.NotFound", "Ürün bulunamadı."));

            try
            {
                product.RemoveAttribute(request.AttributeId);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(Error.NotFound("Product.AttributeNotFound", ex.Message));
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
