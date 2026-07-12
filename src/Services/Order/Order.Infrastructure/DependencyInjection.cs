using AtlasCommerce.BuildingBlocks.EventBus.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Interfaces;
using Order.Infrastructure.EventBus;
using Order.Infrastructure.Persistence;
using Order.Infrastructure.Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // MongoDb
            var mongoSettings = new MongoDbSettings
            {
                ConnectionStrings = configuration["MongoDB:ConnectionString"]
                    ?? "mongodb://atlas:atlas_pass123@localhost:27017",
                DatabaseName = configuration["MongoDB:DatabaseName"] ?? "atlas_orders"
            };

            services.AddSingleton(mongoSettings);
            services.AddSingleton<MongoDbContext>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // RabbitMq + MassTransit
            services.AddEventBus(configuration);
            services.AddScoped<IEventBus, MassTransitEventBus>();

            return services;
        }
    }
}
