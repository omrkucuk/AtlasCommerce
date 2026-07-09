using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Features.Categories.DTOs;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Categories.Queries.GetCategoryById
{
    public sealed class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
    {
        private readonly ICatalogDbContext _context;

        public GetCategoryByIdQueryHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .Where(c => c.Id == request.Id)
                .Select(c => new CategoryDto(
                    c.Id,
                    c.Name,
                    c.Description,
                    c.ImageUrl,
                    c.DisplayOrder,
                    c.IsActive,
                    c.ParentId,
                    c.Parent != null ? c.Parent.Name : null,
                    _context.Categories.Count(sc => sc.ParentId == c.Id)))
                .FirstOrDefaultAsync(cancellationToken);

            if (category is null)
                return Result.Failure<CategoryDto>(Error.NotFound("Category.NotFound", "Kategori bulunamadı"));

            return Result.Success(category);
        }
    }
}
