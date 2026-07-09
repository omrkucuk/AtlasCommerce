using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.UpdateProduct
{
    public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
    {
        private readonly ICatalogDbContext _context;

        public UpdateProductCommandHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product is null)
                return Result.Failure(Error.NotFound("Product.NotFound", "Ürün bulunamadı."));

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

            if (!categoryExists)
                return Result.Failure(Error.NotFound("Category.NotFound", "Kategori bulunamadı."));

            var brandExists = await _context.Brands
                .AnyAsync(b => b.Id == request.BrandId, cancellationToken);

            if (!brandExists)
                return Result.Failure(Error.NotFound("Brand.NotFound", "Marka bulunamadı."));

            product.Update(request.Name, request.Description, request.BasePrice, request.StockQuantity, request.CategoryId, request.BrandId, request.IsActive, request.IsFeatured);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
