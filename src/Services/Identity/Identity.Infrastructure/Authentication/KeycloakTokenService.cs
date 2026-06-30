using Identity.Application.Common.Interfaces;
using Identity.Application.Features.Auth.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Authentication
{
    public sealed class KeycloakTokenService : IKeycloakTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KeycloakTokenService> _logger;

        public KeycloakTokenService(
            HttpClient httpClient, 
            IConfiguration configuration, 
            ILogger<KeycloakTokenService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public Task<KeycloakTokenResult> LoginAsync(string username, string password, CancellationToken cancellationToken)
        {
            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _configuration["Keycloak:ClientId"]!,
                ["client_secret"] = _configuration["Keycloak:ClientSecret"]!,
                ["username"] = username,
                ["password"] = password
            };

            return SendTokenRequestAsync(formData, cancellationToken);
        }

        public Task<KeycloakTokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = _configuration["Keycloak:ClientId"]!,
                ["client_secret"] = _configuration["Keycloak:ClientSecret"]!,
                ["refresh_token"] = refreshToken
            };

            return SendTokenRequestAsync(formData, cancellationToken);
        }

        private async Task<KeycloakTokenResult> SendTokenRequestAsync(Dictionary<string, string> formData, CancellationToken cancellationToken)
        {
            var realm = _configuration["Keycloak:Realm"];
            var requestUri = $"realms/{realm}/protocol/openid-connect/token";

            using var content = new FormUrlEncodedContent(formData);

            try
            {
                var response = await _httpClient.PostAsync(requestUri, content, cancellationToken);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Keycloak token isteği başarısız oldu. Status: {StatusCode}, Body: {Body}",
                        response.StatusCode, responseBody);

                    return new KeycloakTokenResult(false, null, null, 0, "Token alınamadı");
                }

                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                var accessToken = root.GetProperty("access_token").GetString();
                var refreshToken = root.GetProperty("refresh_token").GetString();
                var expiresIn = root.GetProperty("expires_in").GetInt32();

                return new KeycloakTokenResult(true, accessToken, refreshToken, expiresIn, null);
            }
            catch (HttpRequestException ex)
            {

                _logger.LogError(ex, "Keycloak'a bağlanırken hata oluştu");
                return new KeycloakTokenResult(false, null, null, 0, "Kimlik doğrulama servisine ulaşılamadı");
            }
        }
    }
}
