using AtlasCommerce.BuildingBlocks.Common.Middleware;
using Catalog.Application.Features.Categories.Commands.CreateCategory;
using Catalog.Application.Features.Categories.Commands.DeleteCategory;
using Catalog.Application.Features.Categories.Commands.UpdateCategory;
using Catalog.Application.Features.Categories.Queries.GetCategories;
using Catalog.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ISender _mediator;

        public CategoriesController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] Guid? parentId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCategoriesQuery(parentId), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCategoryRequest request,CancellationToken cancellationToken)
        {
            var command = new UpdateCategoryCommand(
                id, request.Name, request.Description,request.DisplayOrder, request.IsActive);

            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
            return result.ToActionResult();
        }
    }

    public sealed record UpdateCategoryRequest(
        string Name,
        string? Description,
        int DisplayOrder,
        bool IsActive);
}
