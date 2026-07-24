using AtlasCommerce.BuildingBlocks.Common.Results;
using AtlasCommerce.BuildingBlocks.EventBus.Events;
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
        private readonly IEventBus _eventBus;
        public UpdateProductCommandHandler(ICatalogDbContext context, IEventBus eventBus)
        {
            _context = context;
            _eventBus = eventBus;
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

            var category = await _context.Categories
                .FirstAsync(c => c.Id == request.CategoryId, cancellationToken);

            var brand = await _context.Brands
                .FirstAsync(b => b.Id == request.BrandId, cancellationToken);

            await _eventBus.PublishAsync(new ProductUpdatedEvent(product.Id, product.Name, product.Sku, product.BasePrice, product.StockQuantity, product.CategoryId, category.Name, product.BrandId, brand.Name, product.IsActive, product.IsFeatured), cancellationToken);

            return Result.Success();
        }
    }
}
