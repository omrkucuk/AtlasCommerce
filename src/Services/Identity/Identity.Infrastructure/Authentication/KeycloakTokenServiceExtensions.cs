using Identity.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Authentication
{
    public static class KeycloakTokenServiceExtensions
    {
        public static IServiceCollection AddKeycloakTokenService(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var authority = configuration["Keycloak:Authority"]
                ?? throw new InvalidOperationException("Keycloak:Authority appsettings.json içinde tanımlı olmalı.");

            var keycloakBaseUri = new Uri(authority).GetLeftPart(UriPartial.Authority) + "/";

            services.AddHttpClient<IKeycloakTokenService, KeycloakTokenService>(client =>
            {
                client.BaseAddress = new Uri(keycloakBaseUri);
            });

            services.AddHttpClient<IKeycloakAdminService, KeycloakAdminService>(client =>
            {
                client.BaseAddress = new Uri(keycloakBaseUri);
            });

            return services;
        }
    }
}
