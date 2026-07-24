using AtlasCommerce.BuildingBlocks.Common.Middleware;
using Identity.Application.Features.Users.Commands.UpdateProfile;
using Identity.Application.Features.Users.Queries.GetCurrentUser;
using Identity.Application.Features.Users.Queries.GetUserPermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ISender _mediator;

        public UsersController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var keycloakId = GetKeycloakIdFromToken();

            if(keycloakId is null)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new GetCurrentUserQuery(keycloakId.Value), cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("me/permissions")]
        public async Task<IActionResult> GetCurrentUserPermissions(CancellationToken cancellationToken)
        {
            var keycloakId = GetKeycloakIdFromToken();

            if (keycloakId is null)
                return Unauthorized();

            var result = await _mediator.Send(new GetUserPermissionsQuery(keycloakId.Value), cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request,CancellationToken cancellationToken)
        {
            var keycloakId = GetKeycloakIdFromToken();
            if (keycloakId is null) return Unauthorized();

            var result = await _mediator.Send(
                new UpdateProfileCommand(
                    keycloakId.Value,
                    request.FirstName,
                    request.LastName,
                    request.Email),
                cancellationToken);

            return result.ToActionResult();
        }

        private Guid? GetKeycloakIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");

            if(claim is null || !Guid.TryParse(claim.Value, out var keycloakId))
            {
                return null;
            }

            return keycloakId;
        }

        public sealed record UpdateProfileRequest(
            string FirstName,
            string LastName,
            string Email);
    }
}
