using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Persistence.Configurations
{
    public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).ValueGeneratedNever();

            builder.Property(v => v.Sku).IsRequired().HasMaxLength(100);
            builder.HasIndex(v => v.Sku).IsUnique();
            builder.Property(v => v.PriceOverride).HasColumnType("decimal(18,2)");

            builder.HasOne<Product>()
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(v => v.Attributes)
                .HasField("_attributes")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
