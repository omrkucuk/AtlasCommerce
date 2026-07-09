using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.AddProductAttribute
{
    public sealed class AddProductAttributeCommandHandler : IRequestHandler<AddProductAttributeCommand, Result<Guid>>
    {
        private readonly ICatalogDbContext _context;

        public AddProductAttributeCommandHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(AddProductAttributeCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(p => p.Attributes)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product is null)
                return Result.Failure<Guid>(Error.NotFound("Product.NotFound", "Ürün bulunamadı."));

            try
            {
                product.AddAttribute(request.Name, request.Value);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<Guid>(Error.Conflict("Product.AttributeExists", ex.Message));
            }

            await _context.SaveChangesAsync(cancellationToken);

            var addedAttribute = product.Attributes
                .First(a => a.Name == request.Name);

            return Result.Success(addedAttribute.Id);
        }
    }
}
