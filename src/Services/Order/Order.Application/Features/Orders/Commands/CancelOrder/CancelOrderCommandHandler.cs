using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using Order.Application.IntegrationEvents;
using Order.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
    {
        private readonly IOrderRepository _repository;
        private readonly IEventBus _eventBus;

        public CancelOrderCommandHandler(IOrderRepository repository, IEventBus eventBus)
        {
            _repository = repository;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
                return Result.Failure(Error.NotFound("Order.NotFound", "Sipariş bulunamadı."));

            try { order.Cancel(request.Reason, request.CancelledBy); }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(Error.Conflict("Order.InvalidOperation", ex.Message));
            }

            await _repository.UpdateAsync(order, cancellationToken);

            foreach (var domainEvent in order.DomainEvents)
            {
                switch (domainEvent)
                {
                    case Order.Domain.Events.OrderCancelledEvent e:
                        await _eventBus.PublishAsync(new OrderCancelledIntegrationEvent(
                            order.Id, order.OrderNumber, e.UserId, e.Reason), cancellationToken);
                        break;
                    case Order.Domain.Events.OrderStatusChangedEvent e:
                        await _eventBus.PublishAsync(new OrderStatusChangedIntegrationEvent(
                            order.Id, order.OrderNumber, e.OldStatus, e.NewStatus), cancellationToken);
                        break;
                }
            }
            order.ClearDomainEvents();

            return Result.Success();
        }
    }
}
