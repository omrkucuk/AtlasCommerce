using AtlasCommerce.BuildingBlocks.EventBus.Events;
using MassTransit;
using Search.API.Services;

namespace Search.API.Consumers.Category
{
    public sealed class CategoryDeletedConsumer : IConsumer<CategoryDeletedEvent>
    {
        private readonly ElasticsearchService _elasticsearchService;
        private readonly ILogger<CategoryDeletedConsumer> _logger;

        public CategoryDeletedConsumer(ElasticsearchService elasticsearchService, ILogger<CategoryDeletedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CategoryDeletedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("CategoryDeleted event alındı. CategoryId: {Id}", evt.CategoryId);
            await _elasticsearchService.DeleteCategoryAsync(evt.CategoryId.ToString(), context.CancellationToken);
        }
    }
}
