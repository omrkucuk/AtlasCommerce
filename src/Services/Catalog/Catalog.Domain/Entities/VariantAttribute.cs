using AtlasCommerce.BuildingBlocks.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public sealed class VariantAttribute : AuditableEntity
    {
        public Guid VariantId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Value { get; private set; } = string.Empty;

        private VariantAttribute() { }

        internal static VariantAttribute Create(Guid variantId, string name, string value)
        {
            return new() { VariantId = variantId, Name = name, Value = value };
        }
    }
}
