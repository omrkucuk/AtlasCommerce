using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using Order.Application.IntegrationEvents;
using Order.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.ConfirmOrder
{
    public sealed class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, Result>
    {
        private readonly IOrderRepository _repository;
        private readonly IEventBus _eventBus;

        public ConfirmOrderCommandHandler(IOrderRepository repository, IEventBus eventBus)
        {
            _repository = repository;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
                return Result.Failure(Error.NotFound("Order.NotFound", "Sipariş bulunamadı."));

            try { order.Confirm(request.TransactionId); }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(Error.Conflict("Order.InvalidOperation", ex.Message));
            }

            await _repository.UpdateAsync(order, cancellationToken);

            foreach (var domainEvent in order.DomainEvents)
            {
                if (domainEvent is Order.Domain.Events.OrderStatusChangedEvent e)
                {
                    await _eventBus.PublishAsync(new OrderStatusChangedIntegrationEvent(
                        order.Id, order.OrderNumber, e.OldStatus, e.NewStatus), cancellationToken);
                }
            }
            order.ClearDomainEvents();

            return Result.Success();
        }
    }
}
