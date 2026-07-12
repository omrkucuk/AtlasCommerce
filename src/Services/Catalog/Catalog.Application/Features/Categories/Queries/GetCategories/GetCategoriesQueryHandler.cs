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

namespace Catalog.Application.Features.Categories.Queries.GetCategories
{
    public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
    {
        private readonly ICatalogDbContext _context;

        public GetCategoriesQueryHandler(ICatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _context.Categories
                .Where(c => c.ParentId == request.ParentId)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
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
                .ToListAsync(cancellationToken);

            return Result.Success<IReadOnlyList<CategoryDto>>(categories);
        }
    }
}
