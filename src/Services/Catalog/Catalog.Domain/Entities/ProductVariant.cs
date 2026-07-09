using AtlasCommerce.BuildingBlocks.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public sealed class ProductVariant : AuditableEntity, ISoftDeletable
    {
        public Guid ProductId { get; private set; }
        public string Sku { get; private set; } = string.Empty;
        public decimal? PriceOverride { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsActive { get; private set; } = true;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }


        private readonly List<VariantAttribute> _attributes = new(); 
        public IReadOnlyCollection<VariantAttribute> Attributes => _attributes.AsReadOnly();

        private ProductVariant() { }

        internal static ProductVariant Create(Guid productId, string sku, decimal? priceOverride, int stockQuantity)
        {
            return new() { 
                ProductId = productId, 
                Sku = sku, 
                PriceOverride = priceOverride, 
                StockQuantity = stockQuantity 
            };
        }

        internal void AddAttribute(string name, string value)
        {
            if (_attributes.Any(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Bu variant'ta '{name}' attribute'u zaten var.");

            _attributes.Add(VariantAttribute.Create(Id, name, value));
        }

        public void UpdateStock(int quantity)
        {
            if (quantity < 0) throw new ArgumentException("Stok miktarı negatif olamaz.");
            StockQuantity = quantity;
        }

        public void UpdatePrice(decimal? priceOverride) => PriceOverride = priceOverride;
    }
}
