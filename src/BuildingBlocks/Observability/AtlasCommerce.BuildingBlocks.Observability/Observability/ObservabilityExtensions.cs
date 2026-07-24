using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;

namespace AtlasCommerce.BuildingBlocks.Observability;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddAtlasObservability(this IServiceCollection services,IConfiguration configuration,
        string serviceName)
    {
        var otlpEndpoint = configuration["Jaeger:Endpoint"] ?? "http://localhost:4317";

        services.AddOpenTelemetry().ConfigureResource(resource => resource
                .AddService(serviceName)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["environment"] = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development",
                    ["version"] = "1.0.0"
                }))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(opts =>
                {
                    opts.Filter = ctx =>
                        !ctx.Request.Path.StartsWithSegments("/health") &&
                        !ctx.Request.Path.StartsWithSegments("/metrics");
                })
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(opts =>
                {
                    opts.Endpoint = new Uri(otlpEndpoint);
                }));

        return services;
    }

    public static WebApplication UseAtlasObservability(this WebApplication app)
    {
        app.UseHttpMetrics();
        app.MapMetrics("/metrics");
        return app;
    } 
}