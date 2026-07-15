using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Gateway.API.Authentication
{
    public static class KeycloakAuthenticationExtensions
    {
        public static IServiceCollection AddKeycloakAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var authority = configuration["Keycloak:Authority"]
                ?? throw new InvalidOperationException("Keycloak:Authority appsettings.json içinde tanımlı olmalı.");

            var audience = configuration["Keycloak:ClientId"]
                ?? throw new InvalidOperationException("Keycloak:ClientId appsettings.json içinde tanımlı olmalı.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;

                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        RoleClaimType = ClaimTypes.Role
                    };

                    // Token doğrulandıktan sonra Keycloak'un nested realm_access.roles claim'ini .Net'in standart Role claim formatına çeviriyoruz
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = KeycloakRoleClaimsTransformer.OnTokenValidated
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}
