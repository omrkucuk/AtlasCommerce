using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Categories.Commands.CreateCategory
{
    public sealed record CreateCategoryCommand(
        string Name,
        string? Description,
        Guid? ParentId,
        int DisplayOrder) : IRequest<Result<Guid>>;
}
