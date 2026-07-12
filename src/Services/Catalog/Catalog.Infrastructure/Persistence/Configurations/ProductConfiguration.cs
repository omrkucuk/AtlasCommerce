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
    public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(300);
            builder.Property(p => p.Sku).IsRequired().HasMaxLength(100);
            builder.HasIndex(p => p.Sku).IsUnique();
            builder.Property(p => p.Description).HasMaxLength(5000);
            builder.Property(p => p.BasePrice).HasColumnType("decimal(18,2)");

            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Navigation(p => p.Images)
                .HasField("_images")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(p => p.Attributes)
                .HasField("_attributes")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(p => p.Variants)
                .HasField("_variants")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
