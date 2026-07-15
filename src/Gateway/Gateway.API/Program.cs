using AtlasCommerce.BuildingBlocks.Common.Middleware;
using AtlasCommerce.BuildingBlocks.HealthChecks;
using AtlasCommerce.BuildingBlocks.Logging;
using Gateway.API.Authentication;
using Gateway.API.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAtlasSerilog(builder.Configuration, serviceName: "ApiGateway");

// YARP — konfigürasyonu
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((_, handler) =>
    {
        handler.SslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
    });

// Keycloak
builder.Services.AddKeycloakAuthentication(builder.Configuration);

// Swagger aggregation
builder.Services.AddGatewaySwagger();

builder.Services.AddHealthChecks();
// Add services to the container.

var app = builder.Build();


app.UseGlobalExceptionHandling();
app.UseAtlasRequestLogging();

if (app.Environment.IsDevelopment())
    app.UseGatewaySwagger();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.MapAtlasHealthChecks();

app.Run();
