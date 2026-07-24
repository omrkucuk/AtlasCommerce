using AtlasCommerce.BuildingBlocks.EventBus.Events;
using MassTransit;
using Search.API.Consumers.Category;
using Search.API.Models;
using Search.API.Services;

namespace Search.API.Consumers.Brand
{
    public sealed class BrandCreatedConsumer : IConsumer<BrandCreatedEvent>
    {
        private readonly ElasticsearchService _elasticsearchService;
        private readonly ILogger<BrandCreatedConsumer> _logger;

        public BrandCreatedConsumer(ElasticsearchService elasticsearchService, ILogger<BrandCreatedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BrandCreatedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("BrandCreated event alındı. BrandId: {Id}", evt.BrandId);

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
