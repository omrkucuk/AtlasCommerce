using AtlasCommerce.BuildingBlocks.EventBus.Events;
using MassTransit;
using Search.API.Models;
using Search.API.Services;

namespace Search.API.Consumers.Brand
{
    public sealed class BrandUpdatedConsumer : IConsumer<BrandUpdatedEvent>
    {
        private readonly ElasticsearchService _elasticsearchService;
        private readonly ILogger<BrandUpdatedConsumer> _logger;

        public BrandUpdatedConsumer(ElasticsearchService elasticsearchService, ILogger<BrandUpdatedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BrandUpdatedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("BrandUpdated event alındı. BrandId: {Id}", evt.BrandId);

            var brandIndex = new BrandIndex
            {
                Id = evt.BrandId.ToString(),
                Name = evt.Name,
                Description = evt.Description,
                IsActive = evt.IsActive,
                LogoUrl = evt.LogoUrl,
            };

            await _elasticsearchService.IndexBrandAsync(brandIndex, context.CancellationToken);
        }
    }
}
