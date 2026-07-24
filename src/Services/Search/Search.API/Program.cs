using AtlasCommerce.BuildingBlocks.Common.Middleware;
using AtlasCommerce.BuildingBlocks.HealthChecks;
using AtlasCommerce.BuildingBlocks.Logging;
using AtlasCommerce.BuildingBlocks.Observability;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Search.API.Authentication;
using Search.API.Consumers.Brand;
using Search.API.Consumers.Category;
using Search.API.Consumers.Products;
using Search.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAtlasSerilog(builder.Configuration, serviceName: "Search.API");

// Elasticsearch
var elasticsearchUrl = builder.Configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
var elasticsearchSettings = new ElasticsearchClientSettings(new Uri(elasticsearchUrl))
    .DefaultIndex("products")
    .RequestTimeout(TimeSpan.FromSeconds(30)); ;

builder.Services.AddSingleton(new ElasticsearchClient(elasticsearchSettings));
builder.Services.AddSingleton<ElasticsearchService>();

// RabbitMQ + MassTransit - consumer
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<ProductCreatedConsumer>();
    cfg.AddConsumer<ProductUpdatedConsumer>();
    cfg.AddConsumer<ProductDeletedConsumer>();
    cfg.AddConsumer<CategoryCreatedConsumer>();
    cfg.AddConsumer<CategoryUpdatedConsumer>();
    cfg.AddConsumer<CategoryDeletedConsumer>();
    cfg.AddConsumer<BrandCreatedConsumer>();
    cfg.AddConsumer<BrandUpdatedConsumer>();
    cfg.AddConsumer<BrandDeletedConsumer>();



    cfg.UsingRabbitMq((ctx, rmq) =>
    {
        rmq.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        rmq.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddKeycloakAuthentication(builder.Configuration);
builder.Services.AddHealthChecks();

builder.Services.AddAtlasObservability(builder.Configuration, serviceName: "Search.API");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Elasticsearch index'lerini uygulama baþlarken oluþtur
var elasticsearchService = app.Services.GetRequiredService<ElasticsearchService>();
await elasticsearchService.EnsureIndicesExistAsync();

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
