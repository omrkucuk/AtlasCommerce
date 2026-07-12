using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Categories.Commands.UpdateCategory
{
    public sealed record UpdateCategoryCommand(
        Guid Id,
        string Name,
        string? Description,
        int DisplayOrder,
        bool IsActive): IRequest<Result>;
}
