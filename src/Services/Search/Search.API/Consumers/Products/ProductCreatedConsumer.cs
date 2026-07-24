using AtlasCommerce.BuildingBlocks.EventBus.Events;
using MassTransit;
using Search.API.Models;
using Search.API.Services;

namespace Search.API.Consumers.Products
{
    public sealed class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
    {
        private readonly ElasticsearchService _elasticsearchService;
        private readonly ILogger<ProductCreatedConsumer> _logger;

        public ProductCreatedConsumer(ElasticsearchService elasticsearchService, ILogger<ProductCreatedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("ProductCreated event alındı. ProductId: {ProductId}", evt.ProductId);

            var productIndex = new ProductIndex
            {
                Id = evt.ProductId.ToString(),
                Name = evt.Name,
                Sku = evt.Sku,
                BasePrice = evt.BasePrice,
                StockQuantity = evt.StockQuantity,
                CategoryId = evt.CategoryId.ToString(),
                CategoryName = evt.CategoryName,
                BrandId = evt.BrandId.ToString(),
                BrandName = evt.BrandName,
                IsActive = evt.IsActive,
                IsFeatured = evt.IsFeatured,
                NameSuggest = evt.Name,
                CreatedAt = DateTime.UtcNow
            };

            await _elasticsearchService.IndexProductAsync(productIndex, context.CancellationToken);
            _logger.LogInformation("Ürün index'lendi. ProductId: {ProductId}", evt.ProductId);
        }
    }
}
