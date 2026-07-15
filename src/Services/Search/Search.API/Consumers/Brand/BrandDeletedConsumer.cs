using AtlasCommerce.BuildingBlocks.EventBus.Events;
using MassTransit;
using Search.API.Services;

namespace Search.API.Consumers.Brand
{
    public sealed class BrandDeletedConsumer : IConsumer<BrandDeletedEvent>
    {
        private readonly ElasticsearchService _elasticsearchService;
        private readonly ILogger<BrandDeletedConsumer> _logger;

        public BrandDeletedConsumer(ElasticsearchService elasticsearchService, ILogger<BrandDeletedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BrandDeletedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("BrandDeleted event alındı. BrandId: {Id}", evt.BrandId);
            await _elasticsearchService.DeleteBrandAsync(evt.BrandId.ToString(), context.CancellationToken);
        }
    }
}
