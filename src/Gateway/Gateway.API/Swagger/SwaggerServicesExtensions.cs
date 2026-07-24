namespace Gateway.API.Swagger
{
    public static class SwaggerServicesExtensions
    {
        // Her downstream servisin Swagger endpoint'i
        private static readonly Dictionary<string, string> _services = new()
        {
            { "Identity Service",  "/swagger-identity/v1/swagger.json" },
            { "Catalog Service",   "/swagger-catalog/v1/swagger.json" },
            { "Order Service",     "/swagger-order/v1/swagger.json" },
            { "Basket Service",    "/swagger-basket/v1/swagger.json" },
            { "Search Service",    "/swagger-search/v1/swagger.json" }
        };

        public static IServiceCollection AddGatewaySwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        public static IApplicationBuilder UseGatewaySwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // Her servis için ayrı dropdown
                foreach (var (name, url) in _services)
                {
                    options.SwaggerEndpoint(url, name);
                }

                options.RoutePrefix = "swagger";
                options.DocumentTitle = "AtlasCommerce API Gateway";
            });

            return app;
        }

    }
}
