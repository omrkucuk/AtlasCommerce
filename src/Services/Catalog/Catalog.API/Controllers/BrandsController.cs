using AtlasCommerce.BuildingBlocks.Common.Middleware;
using Catalog.Application.Features.Brands.Commands.CreateBrand;
using Catalog.Application.Features.Brands.Commands.DeleteBrand;
using Catalog.Application.Features.Brands.Commands.UpdateBrand;
using Catalog.Application.Features.Brands.Queries.GetBrandById;
using Catalog.Application.Features.Brands.Queries.GetBrands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly ISender _mediator;

        public BrandsController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetBrandsQuery(isActive), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetBrandByIdQuery(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBrandCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrandRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateBrandCommand(id, request.Name, request.Description, request.IsActive);
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteBrandCommand(id), cancellationToken);
            return result.ToActionResult();
        }
    }

    public sealed record UpdateBrandRequest(
        string Name,
        string? Description,
        bool IsActive);
}
