using Identity.Application.Features.Users.Commands.SyncUser;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Identity.API.Middleware
{
    public sealed class UserSyncMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserSyncMiddleware> _logger;

        public UserSyncMiddleware(RequestDelegate next, ILogger<UserSyncMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ISender mediator)
        {
            if(context.User.Identity?.IsAuthenticated == true)
            {
                await SyncCurrentUserAsync(context, mediator);
            }

            await _next(context);
        }

        private async Task SyncCurrentUserAsync(HttpContext context, ISender mediator)
        {
            var keycloakIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)
                ?? context.User.FindFirst("sub");

            if(keycloakIdClaim is null || !Guid.TryParse(keycloakIdClaim.Value, out var keycloakId))
            {
                _logger.LogWarning("Token'da geçerli bir 'sub' claim bulunamadı, kullanıcı senkronizasyonu atlandı.");
                return;
            }

            var email = context.User.FindFirst(ClaimTypes.Email)?.Value
                ?? context.User.FindFirst("email")?.Value
                ?? string.Empty;

            var firstName = context.User.FindFirst("given_name")?.Value
                ?? context.User.FindFirst(ClaimTypes.GivenName)?.Value
                ?? string.Empty;

            var lastName = context.User.FindFirst("family_name")?.Value
                ?? context.User.FindFirst(ClaimTypes.Surname)?.Value
                ?? string.Empty;

            var roles = context.User.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var command = new SyncUserCommand(keycloakId, email, firstName, lastName, roles);

            var result = await mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("Kullanıcı senkronizasyonu başarısız oldu. KeycloakId: {KeycloakId}, Hata:{Error}",
                    keycloakId, result.Error.Description);
            }
        }
    }

    public static class UserSyncMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserSync(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UserSyncMiddleware>();
        }
    }
}
