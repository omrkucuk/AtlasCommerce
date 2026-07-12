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
    public sealed class VariantAttributeConfiguration : IEntityTypeConfiguration<VariantAttribute>
    {
        public void Configure(EntityTypeBuilder<VariantAttribute> builder)
        {
            builder.ToTable("VariantAttributes");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedNever();

            builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Value).IsRequired().HasMaxLength(500);

            builder.HasOne<ProductVariant>()
                .WithMany(v => v.Attributes)
                .HasForeignKey(a => a.VariantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
