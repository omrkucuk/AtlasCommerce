using AtlasCommerce.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.IntegrationEvents
{
    public sealed record ProductDeletedEvent(Guid ProductId) : IntegrationEvent;
}
