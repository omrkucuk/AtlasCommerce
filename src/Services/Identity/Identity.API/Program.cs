using AtlasCommerce.BuildingBlocks.Common.Middleware;
using AtlasCommerce.BuildingBlocks.HealthChecks;
using AtlasCommerce.BuildingBlocks.Logging;
using AtlasCommerce.BuildingBlocks.Observability;
using Identity.API.Middleware;
using Identity.Application.Common.Interfaces;
using Identity.Application.Features.Users.Commands.SyncUser;
using Identity.Infrastructure.Authentication;
using Identity.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Logging: Serilog
builder.Host.UseAtlasSerilog(builder.Configuration, serviceName: "Identity.API");

// Authentication: Keycloak JWT doðrulama
builder.Services.AddKeycloakAuthentication(builder.Configuration);

// Auth: Login/Refresh proxy servisi
builder.Services.AddKeycloakTokenService(builder.Configuration);

// Persistence: PostgreSql
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityDb")));

builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<IdentityDbContext>());

// Mediatr:
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(SyncUserCommand).Assembly));


// Health Checks: PostgreSql
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("IdentityDb")!, name: "postgresql");

// Observability
builder.Services.AddAtlasObservability(builder.Configuration, serviceName: "Identity.API");

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
app.UseUserSync();
app.UseAuthorization();

app.MapControllers();
app.MapAtlasHealthChecks();

app.Run();
