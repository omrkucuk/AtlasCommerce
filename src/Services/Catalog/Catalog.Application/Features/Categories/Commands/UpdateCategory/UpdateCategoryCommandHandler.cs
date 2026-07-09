using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Categories.Commands.UpdateCategory
{
    public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
    {
        private readonly ICatalogDbContext _context;

        public UpdateCategoryCommandHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (category is null)
                return Result.Failure(Error.NotFound("Category.NotFound", "Kategori bulunamadı."));

            var nameConflict = await _context.Categories
                .AnyAsync(c => c.Name == request.Name 
                && c.ParentId == category.ParentId 
                && c.Id != request.Id, cancellationToken);

            if (nameConflict)
                return Result.Failure(Error.Conflict("Category.NameExists", "Bu kategori altında aynı isimde başka bir kategori var."));

            category.Update(request.Name, request.Description, request.DisplayOrder, request.IsActive);

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
