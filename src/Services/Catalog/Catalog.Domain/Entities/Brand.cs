using AtlasCommerce.BuildingBlocks.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public sealed class Brand : AuditableEntity, ISoftDeletable
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string? LogoUrl { get; private set; }
        public bool IsActive { get; private set; } = true;

        public bool IsDeleted { get ; set ; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get ; set ; }

        private Brand() { }

        public static Brand Create(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Marka adı boş olamaz.", nameof(name));    
            
            return new Brand { Name = name, Description = description };
        }

        public void Update(string name, string? description, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Marka adı boş olamaz.", nameof(name));

            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public void SetLogo(string logoUrl) => LogoUrl = logoUrl;
    }
}
