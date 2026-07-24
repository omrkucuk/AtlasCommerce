using AtlasCommerce.BuildingBlocks.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public sealed class Category : AuditableEntity, ISoftDeletable
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string? ImageUrl { get; private set; }
        public int DisplayOrder { get; private set; }
        public bool IsActive { get; private set; } = true;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public Guid? ParentId { get; private set; }
        public Category? Parent { get; private set; }

        private readonly List<Category> _subCategories = new();
        public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();

        private Category() { }
        public static Category Create(string name, string? description = null, Guid? parentId = null, int displayOrder = 0)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Kategori adı boş olamaz.", nameof(name));

            return new Category
            {
                Name = name,
                Description = description, 
                ParentId = parentId, 
                DisplayOrder = displayOrder,
            };
        }

        public void Update(string name, string? description, int displayOrder, bool isActive)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Kategori adı boş olamaz", nameof(name));

            Name = name;
            Description = description;
            DisplayOrder = displayOrder;
            IsActive = isActive;
        }

        public void SetImage(string imageUrl) => ImageUrl = imageUrl;
    }
}
