using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasCommerce.BuildingBlocks.EventBus.Extensions
{
    public static class EventBusExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration, Action<IBusRegistrationConfigurator>? configure = null)
        {
            var host = configuration["RabbitMQ:Host"] ?? "localhost";
            var username = configuration["RabbitMQ:Username"] ?? "guest";
            var password = configuration["RabbitMQ:Password"] ?? "guest";

            services.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.UsingRabbitMq((ctx, rmq) =>
                {
                    rmq.Host(host, "/", h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    rmq.ConfigureEndpoints(ctx);
                });
            });

            return services;
        }
    }
}
