using AtlasCommerce.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.IntegrationEvents
{
    public sealed record OrderCancelledIntegrationEvent(
        Guid OrderId,
        string OrderNumber,
        Guid UserId,
        string Reason) : IntegrationEvent;
}
