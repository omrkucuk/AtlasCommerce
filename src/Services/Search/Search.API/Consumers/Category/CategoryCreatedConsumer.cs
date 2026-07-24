using AtlasCommerce.BuildingBlocks.EventBus.Events;
using MassTransit;
using Search.API.Models;
using Search.API.Services;

namespace Search.API.Consumers.Category
{
    public sealed class CategoryCreatedConsumer : IConsumer<CategoryCreatedEvent>
    {
        private readonly ElasticsearchService _elasticsearchService;
        private readonly ILogger<CategoryCreatedConsumer> _logger;

        public CategoryCreatedConsumer(ElasticsearchService elasticsearchService, ILogger<CategoryCreatedConsumer> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CategoryCreatedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("CategoryCreated event alındı. CategoryId: {Id}", evt.CategoryId);

            var categoryIndex = new CategoryIndex
            {
                Id = evt.CategoryId.ToString(),
                Name = evt.Name,
                Description = evt.Description,
                ParentId = evt.ParentId?.ToString(),
                ParentName = evt.ParentName,
                IsActive = evt.IsActive,
                DisplayOrder = evt.DisplayOrder
            };

            await _elasticsearchService.IndexCategoryAsync(categoryIndex, context.CancellationToken);
        }
    }
}
