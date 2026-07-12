using Order.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.Entities
{
    public sealed class OrderItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public string? ProductImageUrl { get; private set; }
        public string Sku { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public Money UnitPrice { get; private set; } = Money.Zero();
        public Money TotalPrice => UnitPrice.Multiply(Quantity);

        private OrderItem() { }

        internal static OrderItem Create(Guid productId, string productName, string sku, int quantity, Money unitPrice, string? imageUrl = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Ürün miktarı sıfırdan büyük olmalı.");

            return new OrderItem
            {
                ProductId = productId,
                ProductName = productName,
                Sku = sku,
                Quantity = quantity,
                UnitPrice = unitPrice,
                ProductImageUrl = imageUrl
            };
        }
    }
}
