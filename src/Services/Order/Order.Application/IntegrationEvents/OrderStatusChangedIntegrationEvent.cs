using AtlasCommerce.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.IntegrationEvents
{
    public sealed record OrderStatusChangedIntegrationEvent(
        Guid OrderId,
        string OrderNumber,
        string OldStatus,
        string NewStatus) : IntegrationEvent;
}
