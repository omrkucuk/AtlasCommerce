using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text.Json;

namespace Search.API.Authentication
{
    public static class KeycloakRoleClaimsTransformer
    {
        public static Task OnTokenValidated(TokenValidatedContext context)
        {
            var realmAccessClaim = context.Principal?.FindFirst("realm_access");

            if (realmAccessClaim is null)
                return Task.CompletedTask;

            using var doc = JsonDocument.Parse(realmAccessClaim.Value);

            if (!doc.RootElement.TryGetProperty("roles", out var rolesElement))
                return Task.CompletedTask;

            var identity = context.Principal!.Identity as ClaimsIdentity;

            foreach (var role in rolesElement.EnumerateArray())
            {
                var roleName = role.GetString();
                if (!string.IsNullOrEmpty(roleName))
                {
                    identity?.AddClaim(new Claim(ClaimTypes.Role, roleName));
                }
            }

            return Task.CompletedTask;
        }
    }
}
