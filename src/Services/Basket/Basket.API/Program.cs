using AtlasCommerce.BuildingBlocks.Common.Middleware;
using AtlasCommerce.BuildingBlocks.HealthChecks;
using AtlasCommerce.BuildingBlocks.Logging;
using AtlasCommerce.BuildingBlocks.Observability;
using Basket.API.Authentication;
using Basket.API.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAtlasSerilog(builder.Configuration, serviceName: "Basket.API");

// Redis
var redisConnection = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));

// Servisler
builder.Services.AddScoped<IBasketService, RedisBasketService>();

// Order Service HTTP client
var orderServiceUrl = builder.Configuration["OrderService:BaseUrl"] ?? "https://localhost:7045";
builder.Services.AddHttpClient<IOrderService, OrderServiceClient>(client =>
{
    client.BaseAddress = new Uri(orderServiceUrl);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.AddKeycloakAuthentication(builder.Configuration);

builder.Services.AddHealthChecks().AddRedis(redisConnection, name: "redis");

builder.Services.AddAtlasObservability(builder.Configuration, serviceName: "Basket.API");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseGlobalExceptionHandling();
app.UseAtlasRequestLogging();
app.UseAtlasObservability();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapAtlasHealthChecks();

app.Run();
