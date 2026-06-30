using Identity.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Authentication
{
    public sealed class KeycloakAdminService : IKeycloakAdminService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KeycloakAdminService> _logger;

        public KeycloakAdminService(HttpClient httpClient, IConfiguration configuration, ILogger<KeycloakAdminService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<KeycloakAdminResult> CreateUserAsync(string username, string email, string password, string firstName, string lastName, CancellationToken cancellationToken)
        {
            var adminToken = await GetAdminAccessTokenAsync(cancellationToken);

            if (adminToken is null)
            {
                return new KeycloakAdminResult(false, null, "Admin yetkisi alınamadı.");
            }

            var realm = _configuration["Keycloak:Realm"];

            var payload = new
            {
                username,
                email,
                firstName,
                lastName,
                enabled = true,
                emailVerified = true,
                credentials = new[]
                {
                    new {type="password", value=password, temporary=false}
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, $"admin/realms/{realm}/users")
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            try
            {
                var response = await _httpClient.SendAsync(request, cancellationToken);

                if(response.StatusCode == HttpStatusCode.Conflict)
                {
                    return new KeycloakAdminResult(false, null, "Bu kullanıcı adı veya email zaten kayıtlı.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("Keycloak kullanıcı oluşturma başarısız. Status: {StatusCode}, Body: {Body}",
                        response.StatusCode, errorBody);

                    return new KeycloakAdminResult(false, null, "Kullanıcı oluşturulamadı");
                }

                var locationHeader = response.Headers.Location;
                Guid? newUserId = null;

                if(locationHeader is not null)
                {
                    var segments = locationHeader.Segments;
                    var idSegment = segments[^1].TrimEnd('/');
                    if(Guid.TryParse(idSegment, out var parseId))
                    {
                        newUserId = parseId;
                    }
                }

                return new KeycloakAdminResult(true, newUserId, null);

            }
            catch (HttpRequestException ex)
            {

                _logger.LogError(ex, "Keycloak Admin API'sine bağlanırken hata oluştu.");
                return new KeycloakAdminResult(false, null, "Kimlik doğrulama servisine ulaşılamadı");
            }
        }

        private async Task<string>? GetAdminAccessTokenAsync(CancellationToken cancellationToken)
        {
            var realm = _configuration["Keycloak:Realm"];
            var clientId = _configuration["Keycloak:ClientId"];
            var clientSecret = _configuration["Keycloak:ClientSecret"];

            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = clientId!,
                ["client_secret"] = clientSecret!
            };

            using var content = new FormUrlEncodedContent(formData);

            try
            {
                var response = await _httpClient.PostAsync($"realms/{realm}/protocol/openid-connect/token", content, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("Admin token alınamadı. Status: {StatusCode}, Body: {Body}", response.StatusCode, errorBody);
                    return null;
                }

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(responseBody);

                return doc.RootElement.GetProperty("access_token").GetString();
            }
            catch (HttpRequestException ex)
            {

                _logger.LogError(ex, "Keycloak'tan admin token alınırken hata oluştu.");
                return null;
            }
        }
    }
}
