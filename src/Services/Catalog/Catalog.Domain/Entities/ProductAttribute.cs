using AtlasCommerce.BuildingBlocks.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public sealed class ProductAttribute : AuditableEntity
    {
        public Guid ProductId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Value { get; private set; } = string.Empty;

        private ProductAttribute() { }

        internal static ProductAttribute Create(Guid productId, string name, string value)
        {
            return new ProductAttribute() { ProductId = productId, Name = name, Value = value };
        }
    }
}
