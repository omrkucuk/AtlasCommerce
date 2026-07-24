using AtlasCommerce.BuildingBlocks.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public sealed class Product : AuditableEntity, ISoftDeletable
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string Sku { get; private set; } = string.Empty;
        public decimal BasePrice { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsActive { get; private set; } = true;
        public bool IsFeatured { get; private set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public Guid CategoryId { get; private set; }
        public Category? Category { get; private set; }
        public Guid BrandId { get; private set; }
        public Brand? Brand { get; private set; }

        private readonly List<ProductImage> _images = new();
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

        private readonly List<ProductAttribute> _attributes = new();
        public IReadOnlyCollection<ProductAttribute> Attributes => _attributes.AsReadOnly();

        private readonly List<ProductVariant> _variants = new();
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

        private Product() { }
        public static Product Create(string name, string? description, string sku, decimal basePrice, int stockQuantity, Guid categoryId, Guid brandId)
        {
            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentException("Ürün adı boş olamaz.", nameof(name));
            if (string.IsNullOrWhiteSpace(sku)) 
                throw new ArgumentException("SKU boş olamaz.", nameof(sku));
            if (basePrice < 0) 
                throw new ArgumentException("Fiyat negatif olamaz.", nameof(basePrice));
            if (stockQuantity < 0) 
                throw new ArgumentException("Stok miktarı negatif olamaz.", nameof(stockQuantity));

            return new Product
            {
                Name = name,
                Description = description,
                Sku = sku,
                BasePrice = basePrice,
                StockQuantity = stockQuantity,
                CategoryId = categoryId,
                BrandId = brandId
            };
        }

        public void Update(string name, string? description, decimal basePrice, int stockQuantity, Guid categoryId, Guid brandId, bool isActive, bool isFeatured)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ürün adı boş olamaz.", nameof(name));
            if (basePrice < 0)
                throw new ArgumentException("Fiyat negatif olamaz.", nameof(basePrice));
            if (stockQuantity < 0) 
                throw new ArgumentException("Stok miktarı negatif olamaz.", nameof(stockQuantity));

            Name = name;
            Description = description;
            BasePrice = basePrice;
            StockQuantity = stockQuantity;
            CategoryId = categoryId;
            BrandId = brandId;
            IsActive = isActive;
            IsFeatured = isFeatured;
        }

        // Görsel yönetimi
        public void AddImage(string imageUrl, bool isMain = false)
        {
            if (string.IsNullOrEmpty(imageUrl))
                throw new ArgumentException("Görsel URL boş olamaz.", nameof(imageUrl));

            if (isMain)
                foreach (var img in _images)
                    img.SetMain(false);

            var shouldBeMain = isMain || !_images.Any();
            _images.Add(ProductImage.Create(Id, imageUrl, shouldBeMain));
        }

        public void RemoveImage(Guid imageId)
        {
            var image = _images.FirstOrDefault(i => i.Id == imageId)
                ?? throw new InvalidOperationException("Görsel bulunamadı");
            _images.Remove(image);

            if (image.IsMain && _images.Any())
                _images.First().SetMain(true);
        }

        // Attribute 
        public void AddAttribute(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Attribute adı boş olamaz.", nameof(name));

            if (_attributes.Any(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"'{name}' adında bir attribute zaten var.");

            _attributes.Add(ProductAttribute.Create(Id, name, value));
        }

        public void RemoveAttribute(Guid attributeId)
        {
            var attribute = _attributes.FirstOrDefault(a => a.Id == attributeId)
                ?? throw new InvalidOperationException("Attribute bulunamadı.");

            _attributes.Remove(attribute);
        }

        // Variant
        public void AddVariant(string sku, decimal? priceOverride, int stockQuantity, List<(string Name, string Value)> attributes)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("Variant SKU boş olamaz.", nameof(sku));
            if (_variants.Any(v => v.Sku.Equals(sku, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"'{sku}' SKU'lu bir variant zaten var.");

            var variant = ProductVariant.Create(Id, sku, priceOverride, stockQuantity);

            foreach (var (attrName, attrValue) in attributes)
                variant.AddAttribute(attrName, attrValue);

            _variants.Add(variant);
        }

        public void RemoveVariant(Guid variantId)
        {
            var variant = _variants.FirstOrDefault(v => v.Id == variantId)
                ?? throw new InvalidOperationException("Variant bulunamadı");

            _variants.Remove(variant);
        }

        // Stok Yönetimi
        public void IncreaseStock(int quantity)
        {
            if (quantity < 0) throw new ArgumentException("Miktar pozitif olmalı.");
            StockQuantity += quantity;
        }

        public void DecreaseStock(int quantity)
        {
            if (quantity < 0) throw new ArgumentException("Miktar pozitif olmalı.");
            if (StockQuantity < quantity) throw new InvalidOperationException("Yetersiz stok.");
            StockQuantity -= quantity;
        }
    }
}
