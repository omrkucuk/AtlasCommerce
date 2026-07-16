using AtlasCommerce.BuildingBlocks.Common.Middleware;
using AtlasCommerce.BuildingBlocks.HealthChecks;
using AtlasCommerce.BuildingBlocks.Logging;
using AtlasCommerce.BuildingBlocks.Observability;
using Order.Application;
using Order.Infrastructure;
using Order.Infrastructure.Authentication;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseAtlasSerilog(builder.Configuration, serviceName: "Order.API");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddKeycloakAuthentication(builder.Configuration);

builder.Services.AddAtlasObservability(builder.Configuration, serviceName: "Order.API");

builder.Services.AddHealthChecks();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

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
