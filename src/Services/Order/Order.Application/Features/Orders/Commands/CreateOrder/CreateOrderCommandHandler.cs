using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using Order.Application.IntegrationEvents;
using Order.Application.Interfaces;
using Order.Domain.Enums;
using Order.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.CreateOrder
{
    public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<string>>
    {
        private readonly IOrderRepository _repository;
        private readonly IEventBus _eventBus;

        public CreateOrderCommandHandler(IOrderRepository repository, IEventBus eventBus)
        {
            _repository = repository;
            _eventBus = eventBus;
        }

        public async Task<Result<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (!request.Items.Any())
                return Result.Failure<string>(Error.Validation("Order.NoItems", "Sipariş en az bir ürün içermeli."));

            if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var paymentMethod))
                return Result.Failure<string>(Error.Validation("Order.InvalidPaymentMethod", "Geçersiz ödeme yöntemi."));

            var shippingAddress = Address.Of(
                request.ShippingAddress.FirstName, request.ShippingAddress.LastName, request.ShippingAddress.Phone,
                request.ShippingAddress.City, request.ShippingAddress.District, request.ShippingAddress.FullAddress,
                request.ShippingAddress.ZipCode, request.ShippingAddress.Country);

            var billingAddress = Address.Of(
                request.BillingAddress.FirstName, request.BillingAddress.LastName,
                request.BillingAddress.Phone, request.BillingAddress.City,
                request.BillingAddress.District, request.BillingAddress.FullAddress,
                request.BillingAddress.ZipCode, request.BillingAddress.Country);

            var order = Order.Domain.Entities.Order.Create(
                request.UserId,
                shippingAddress,
                billingAddress,
                paymentMethod,
                Money.Of(request.ShippingFee));

            foreach (var item in request.Items)
            {
                order.AddItem(
                    item.ProductId, item.ProductName, item.Sku, item.Quantity, Money.Of(item.UnitPrice), item.ImageUrl);
            }

            await _repository.AddAsync(order, cancellationToken);

            // Domain event'leri integration event'e çevir
            await PublishDomainEventsAsync(order, cancellationToken);

            return Result.Success(order.OrderNumber);
        }

        private async Task PublishDomainEventsAsync(Order.Domain.Entities.Order order, CancellationToken cancellationToken)
        {
            foreach (var domainEvent in order.DomainEvents)
            {
                switch (domainEvent)
                {
                    case Order.Domain.Events.OrderCreatedEvent e:
                        await _eventBus.PublishAsync(new OrderCreatedIntegrationEvent(
                            e.OrderId, order.OrderNumber, e.UserId,
                            order.TotalAmount.Amount, order.TotalAmount.Currency),
                            cancellationToken);
                        break;
                }
            }
            order.ClearDomainEvents();
        }
    }
}
