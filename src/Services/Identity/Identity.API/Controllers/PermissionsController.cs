using AtlasCommerce.BuildingBlocks.Common.Middleware;
using Identity.Application.Features.Permissions.Commands.CreatePermission;
using Identity.Application.Features.Permissions.Queries.GetAllPermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PermissionsController : ControllerBase
    {
        private readonly ISender _mediator;

        public PermissionsController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllPermissionsQuery(), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePermissionCommand request,CancellationToken
            cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return result.ToActionResult();
        }
    }
}
