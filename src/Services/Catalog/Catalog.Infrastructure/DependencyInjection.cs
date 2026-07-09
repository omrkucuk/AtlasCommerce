using AtlasCommerce.BuildingBlocks.Common.Abstract;
using AtlasCommerce.BuildingBlocks.Common.Interceptors;
using AtlasCommerce.BuildingBlocks.EventBus.Extensions;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence.Context;
using Catalog.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddScoped<AuditInterceptor<CatalogDbContext, CatalogAuditLog>>();
            services.AddScoped<SoftDeleteInterceptor>();

            services.AddDbContext<CatalogDbContext>((sp, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CatalogDb"));

                options.AddInterceptors(
                    sp.GetRequiredService<AuditInterceptor<CatalogDbContext, CatalogAuditLog>>(),
                    sp.GetRequiredService<SoftDeleteInterceptor>());
            });

            services.AddScoped<ICatalogDbContext>(sp =>
                sp.GetRequiredService<CatalogDbContext>());

            services.AddMinio(client => client
                .WithEndpoint(configuration["MinIO:Endpoint"] ?? "localhost:9000")
                .WithCredentials(
                    configuration["MinIO:AccessKey"] ?? "atlas", 
                    configuration["MinIO:SecretKey"] ?? "atlas_pass123")
                .WithSSL(false)
                .Build());
                

            services.AddScoped<IFileStorageService, MinioFileStorageService>();

            services.AddEventBus(configuration);

            return services;
        }
    }
}
