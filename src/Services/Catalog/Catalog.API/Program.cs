using AtlasCommerce.BuildingBlocks.Common.Middleware;
using AtlasCommerce.BuildingBlocks.HealthChecks;
using AtlasCommerce.BuildingBlocks.Logging;
using AtlasCommerce.BuildingBlocks.Observability;
using Catalog.Application;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAtlasSerilog(builder.Configuration, serviceName: "Catalog.API");

// Add services to the container.

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddKeycloakAuthentication(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("CatalogDb")!, name: "mssql");

builder.Services.AddAtlasObservability(builder.Configuration, serviceName: "Catalog.API");

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

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapAtlasHealthChecks();

app.Run();
