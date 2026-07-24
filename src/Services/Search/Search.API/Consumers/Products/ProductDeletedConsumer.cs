using AtlasCommerce.BuildingBlocks.EventBus.Events;
using MassTransit;
using Search.API.Services;

namespace Search.API.Consumers.Products
{
    public sealed class ProductDeletedConsumer : IConsumer<ProductDeletedEvent>
    {
        private readonly ElasticsearchService _elasticsearchService;
        private readonly ILogger<ProductDeletedConsumer> _logger;

        public ProductDeletedConsumer(ElasticsearchService elasticsearchService, ILogger<ProductDeletedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("ProductDeleted event alındı. ProductId: {ProductId}", evt.ProductId);

            await _elasticsearchService.DeleteProductAsync(
                evt.ProductId.ToString(), context.CancellationToken);

            _logger.LogInformation("Ürün index'ten silindi. ProductId: {ProductId}", evt.ProductId);
        }
    }
}
