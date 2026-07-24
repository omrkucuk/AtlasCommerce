using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Categories.DTOs
{
    public sealed record CategoryDto
    (
        Guid Id,
        string Name,
        string? Description,
        string? ImageUrl,
        int DisplayOrder,
        bool IsActive,
        Guid? ParentId,
        string? ParentName,
        int SubCategoryCount);
}
