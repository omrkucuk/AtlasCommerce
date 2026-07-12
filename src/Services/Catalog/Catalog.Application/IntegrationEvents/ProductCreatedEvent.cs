using AtlasCommerce.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.IntegrationEvents
{
    public sealed record ProductCreatedEvent(
        Guid ProductId,
        string Name,
        string Sku,
        decimal BasePrice,
        int StockQuantity,
        Guid CategoryId,
        string CategoryName,
        Guid BrandId,
        string BrandName,
        bool IsActive,
        bool IsFeatured) : IntegrationEvent;
}
