using AtlasCommerce.BuildingBlocks.Common.Middleware;
using AtlasCommerce.BuildingBlocks.HealthChecks;
using AtlasCommerce.BuildingBlocks.Logging;
using AtlasCommerce.BuildingBlocks.Observability;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("StoreFront", policy =>
    {
        policy
            .WithOrigins("https://localhost:5173", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Swagger aggregation
builder.Services.AddGatewaySwagger();

builder.Services.AddAtlasObservability(builder.Configuration, serviceName: "ApiGateway");

builder.Services.AddHealthChecks();

builder.Services.AddHealthChecksUI(opts =>
{
    opts.SetEvaluationTimeInSeconds(10);
    opts.MaximumHistoryEntriesPerEndpoint(50);
    opts.UseApiEndpointHttpMessageHandler(sp => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });
})
.AddInMemoryStorage();


// Add services to the container.

var app = builder.Build();

app.UseCors("StoreFront");

app.UseGlobalExceptionHandling();
app.UseAtlasRequestLogging();
app.UseAtlasObservability();

if (app.Environment.IsDevelopment())
    app.UseGatewaySwagger();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapHealthChecksUI(opts =>
{
    opts.UIPath = "/health-ui";
    opts.ApiPath = "/health-ui-api";
});

app.MapReverseProxy();
app.MapAtlasHealthChecks();

app.Run();
