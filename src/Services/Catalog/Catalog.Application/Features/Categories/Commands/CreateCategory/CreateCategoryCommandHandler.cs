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

namespace Catalog.Application.Features.Categories.Commands.CreateCategory
{
    public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
    {
        private readonly ICatalogDbContext _context;

        public CreateCategoryCommandHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var nameExists = await _context.Categories
                .AnyAsync(c => c.Name == request.Name && c.ParentId == request.ParentId, cancellationToken);

            if (nameExists)
                return Result.Failure<Guid>(Error.Conflict("Category.NameExists", $"Bu kategori altında '{request.Name}' adında bir kategori zaten var."));

            if (request.ParentId.HasValue)
            {
                var parentExists = await _context.Categories
                    .AnyAsync(c => c.Id == request.ParentId.Value, cancellationToken);

                if (!parentExists)
                    return Result.Failure<Guid>(Error.NotFound("Category.NotFound", "Üst kategori bulunamadı."));
            }

            var category = Category.Create(request.Name, request.Description, request.ParentId, request.DisplayOrder);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(category.Id);
        }
    }
}
