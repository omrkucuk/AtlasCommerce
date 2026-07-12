using AtlasCommerce.BuildingBlocks.Common.Middleware;
using Identity.Application.Features.Roles.Commands.AssignPermission;
using Identity.Application.Features.Roles.Commands.CreateRole;
using Identity.Application.Features.Roles.Commands.RevokePermission;
using Identity.Application.Features.Roles.Dtos;
using Identity.Application.Features.Roles.Queries.GetAllRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly ISender _mediator;

        public RolesController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllRolesQuery(), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{roleId:guid}/permissions/{permissionId:guid}")]
        public async Task<IActionResult> AssignPermission(
            Guid roleId,
            Guid permissionId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new AssignPermissionCommand(roleId, permissionId), cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{roleId:guid}/permissions/{permissionId:guid}")]
        public async Task<IActionResult> RevokePermission(
            Guid roleId,
            Guid permissionId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RevokePermissionCommand(roleId, permissionId), cancellationToken);
            return result.ToActionResult();
        }
    }
}
