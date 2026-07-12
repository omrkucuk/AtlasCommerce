using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Categories.Commands.DeleteCategory
{
    public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly ICatalogDbContext _context;

        public DeleteCategoryCommandHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (category is null)
                return Result.Failure(Error.NotFound("Category.NotFound", "Kategori bulunamadı."));

            var hasSubCategories = await _context.Categories
                .AnyAsync(c => c.ParentId == request.Id, cancellationToken);

            if (hasSubCategories)
                return Result.Failure(Error.Conflict("Category.HasSubCategories", "Alt kategorileri olan bir kategori silinemez. Önce alt kategorileri silin."));

            var hasProducts = await _context.Products
                .AnyAsync(p => p.CategoryId == request.Id, cancellationToken);

            if (hasProducts)
                return Result.Failure(Error.Conflict("Category.HasProducts", "Ürünleri olan bir kategori silinemez."));

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
