using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.CreateProduct
{
    public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        private readonly ICatalogDbContext _context;

        public CreateProductCommandHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var skuExists = await _context.Products
                .AnyAsync(p => p.Sku == request.Sku, cancellationToken);

            if (skuExists)
                return Result.Failure<Guid>(Error.Conflict("Product.SkuExists", $"'{request.Sku}' SKU'lu bir ürün zaten var."));

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

            if (!categoryExists)
                return Result.Failure<Guid>(Error.NotFound("Category.NotFound", "Kategori bulunamadı."));

            var brandExists = await _context.Brands
                .AnyAsync(b => b.Id == request.BrandId, cancellationToken);

            if (!brandExists)
                return Result.Failure<Guid>(Error.NotFound("Brand.NotFound", "Marka bulunamadı."));

            var product = Product.Create(
                request.Name,
                request.Description,
                request.Sku,
                request.BasePrice,
                request.StockQuantity,
                request.CategoryId,
                request.BrandId);

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(product.Id);
        }
    }
}
