using Catalog.Application.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.EventBus
{
    public sealed class MassTransitEventBus : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitEventBus(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken) where T : class
        {
            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        }
    }
}
