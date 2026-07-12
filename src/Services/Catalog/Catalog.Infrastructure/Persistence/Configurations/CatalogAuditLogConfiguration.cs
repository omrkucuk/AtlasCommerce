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
    public sealed class CatalogAuditLogConfiguration : IEntityTypeConfiguration<CatalogAuditLog>
    {
        public void Configure(EntityTypeBuilder<CatalogAuditLog> builder)
        {
            builder.ToTable("AuditLogs");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.EntityName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.EntityId).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Action).IsRequired().HasMaxLength(50);
            builder.Property(a => a.ChangedBy).HasMaxLength(100);
        }
    }
}
