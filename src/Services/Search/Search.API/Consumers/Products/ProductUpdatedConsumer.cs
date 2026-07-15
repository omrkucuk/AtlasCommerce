using AtlasCommerce.BuildingBlocks.EventBus.Events;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Search.API.Models;
using Search.API.Services;

namespace Search.API.Consumers.Products
{
    public sealed class ProductUpdatedConsumer : IConsumer<ProductUpdatedEvent>
    {
        private readonly ElasticsearchService _elasticsearchService;
        private readonly ILogger<ProductUpdatedConsumer> _logger;

        public ProductUpdatedConsumer(ElasticsearchService elasticsearchService, ILogger<ProductUpdatedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("ProductUpdated event alındı. ProductId: {ProductId}", evt.ProductId);

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
                UpdatedAt = DateTime.UtcNow
            };

            await _elasticsearchService.IndexProductAsync(productIndex, context.CancellationToken);
            _logger.LogInformation("Ürün index güncellendi. ProductId: {ProductId}", evt.ProductId);
        }
    }
}
