using Order.Domain.Enums;
using Order.Domain.Events;
using Order.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.Entities
{
    public sealed class Order
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; }
        public string OrderNumber { get; private set; } = string.Empty;
        public OrderStatus Status { get; private set; }
        public Address ShippingAddress { get; private set; } = null!;
        public Address BillingAddress { get; private set; } = null!;
        public PaymentInfo PaymentInfo { get; private set; } = null!;
        public string? CargoTrackingNumber { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }


        private List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private List<OrderNote> _notes = new();
        public IReadOnlyCollection<OrderNote> Notes => _notes.AsReadOnly();

        public Money SubTotal => _items.Aggregate(Money.Zero(), (acc, item) => acc.Add(item.TotalPrice));
        public Money ShippingFee { get; private set; } = Money.Zero();
        public Money TotalAmount => SubTotal.Add(ShippingFee);

        private List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        public void ClearDomainEvents() => _domainEvents.Clear();

        private Order() { }

        public static Order Create(
            Guid userId, 
            Address shippingAddress, 
            Address billingAddress, 
            PaymentMethod paymentMethod, 
            Money? shippingFee = null)
        {
            if (userId == Guid.Empty) throw new ArgumentException("UserId boş olamaz.");

            var order = new Order
            {
                UserId = userId,
                OrderNumber = GenerateOrderNumber(),
                Status = OrderStatus.Pending,
                ShippingAddress = shippingAddress,
                BillingAddress = billingAddress,
                PaymentInfo = PaymentInfo.Pending(paymentMethod),
                ShippingFee = shippingFee ?? Money.Zero(),
                CreatedAt = DateTime.UtcNow,
            };

            // Sipariş oluşturma sonrası domain event ekleme
            order._domainEvents.Add(new OrderCreatedEvent(order.Id, userId, 0));

            return order;
        }

        // Ürün Yönetimi
        public void AddItem(Guid productId, string productName, string sku, int quantity, Money unitPrice, string? imageUrl = null)
        {
            EnsureOrderIsModifiable();

            var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
            if(existingItem is not null)
            {
                _items.Remove(existingItem);
                _items.Add(OrderItem.Create(productId, productName, sku, 
                    existingItem.Quantity + quantity, unitPrice, imageUrl));
            }
            else
            {
                _items.Add(OrderItem.Create(productId, productName, sku, quantity, unitPrice, imageUrl));
            }

            Touch();
        }

        public void RemoveItem(Guid productId)
        {
            EnsureOrderIsModifiable();

            var item = _items.FirstOrDefault(i => i.ProductId == productId)
                ?? throw new InvalidOperationException("Ürün siparişte bulunamadı.");

            _items.Remove(item);

            if (!_items.Any())
                throw new InvalidOperationException("Sipariş en az bir ürün içermeli.");

            Touch();
        }

        // Durum Geçişleri
        public void Confirm(string transactionId)
        {
            EnsureStatus(OrderStatus.Pending, "Sadece Pending siparişler onaylanabilir.");

            if (!_items.Any())
                throw new InvalidOperationException("Boş sipariş onaylanamaz.");

            var oldStatus = Status.ToString();
            Status = OrderStatus.Confirmed;
            PaymentInfo = PaymentInfo.MarkAsPaid(transactionId);

            _domainEvents.Add(new OrderStatusChangedEvent(Id, oldStatus, Status.ToString()));
            Touch();
        }

        public void Ship(string cargoTrackingNumber)
        {
            EnsureStatus(OrderStatus.Confirmed, "Sadece Confirmed siparişler kargoya verilebilir.");

            if (string.IsNullOrWhiteSpace(cargoTrackingNumber))
                throw new ArgumentException("Kargo takip numarası boş olamaz.");

            var oldStatus = Status.ToString();
            Status = OrderStatus.Shipped;
            CargoTrackingNumber = cargoTrackingNumber;

            _domainEvents.Add(new OrderStatusChangedEvent(Id, oldStatus, Status.ToString()));
            Touch();
        }

        public void Deliver()
        {
            EnsureStatus(OrderStatus.Delivered, "Sadece Shipped siparişler teslim edilebilir.");

            var oldStatus = Status.ToString();
            Status = OrderStatus.Delivered;

            _domainEvents.Add(new OrderStatusChangedEvent(Id, oldStatus, Status.ToString()));
        }

        public void Cancel(string reason, string cancelledBy)
        {
            if (Status is OrderStatus.Delivered)
                throw new InvalidOperationException("Teslim edilen sipariş iptal edilemez.");

            if (Status is OrderStatus.Cancelled)
                throw new InvalidOperationException("Sipariş zaten iptal edilmiş.");

            var oldStatus = Status.ToString();
            Status = OrderStatus.Cancelled;

            if (PaymentInfo.Status == PaymentStatus.Paid)
                PaymentInfo = PaymentInfo.MarkAsRefunded();

            _notes.Add(OrderNote.Create($"İptal Sebebi: {reason}", cancelledBy));
            _domainEvents.Add(new OrderCancelledEvent(Id, UserId, reason));
            _domainEvents.Add(new OrderStatusChangedEvent(Id, oldStatus, Status.ToString()));
            Touch();
        }


        // Not Ekleme
        public void AddNote(string content, string addedBy, bool isCustomerVisible = false)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Not içeriği boş olamaz.");

            _notes.Add(OrderNote.Create(content, addedBy, isCustomerVisible));
            Touch();
        }


        // Helpers
        private void EnsureOrderIsModifiable()
        {
            if (Status is OrderStatus.Cancelled or OrderStatus.Delivered)
                throw new InvalidOperationException($"'{Status}' durumundaki sipariş değiştirilemez.");
        }

        private void EnsureStatus(OrderStatus expected, string message)
        {
            if (Status != expected)
                throw new InvalidOperationException(message);
        }

        private void Touch() => UpdatedAt = DateTime.UtcNow;


        private static string GenerateOrderNumber()
        {
            var now = DateTime.UtcNow;
            return $"ORD-{now:yyyyMM}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
    }
}
