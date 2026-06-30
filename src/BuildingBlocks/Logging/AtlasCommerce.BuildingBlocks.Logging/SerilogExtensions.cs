using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasCommerce.BuildingBlocks.Logging
{
    public static class SerilogExtensions
    {
        public static IHostBuilder UseAtlasSerilog(
            this IHostBuilder hostBuilder, 
            IConfiguration configuration, 
            string serviceName)
        {
            return hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
            {
                var seqServerUrl = configuration["Seq:ServerUrl"] ?? "http://localhost:5341";

                loggerConfiguration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ServiceName", serviceName)
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj}{NewLine} {Exception}")
                    .WriteTo.Seq(seqServerUrl);
            });
        }
    }
}
