using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Features.Categories.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Categories.Queries.GetCategories
{
    public sealed record GetCategoriesQuery(Guid? ParentId = null) : IRequest<Result<IReadOnlyList<CategoryDto>>>;
}
