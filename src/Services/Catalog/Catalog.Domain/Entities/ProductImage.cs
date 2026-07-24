using AtlasCommerce.BuildingBlocks.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public class ProductImage : AuditableEntity
    {
        public Guid ProductId { get; private set; }
        public string ImageUrl { get; private set; } = string.Empty;
        public bool IsMain { get; private set; }
        public int DisplayOrder { get; private set; }

        private ProductImage() { }

        internal static ProductImage Create(Guid productId, string imageUrl, bool isMain = false)
        {
            return new () { ProductId = productId, ImageUrl = imageUrl, IsMain = isMain };
        }
        
        internal void SetMain(bool isMain) => IsMain = isMain;
        internal void SetOrder(int order) => DisplayOrder = order;
    }
}
