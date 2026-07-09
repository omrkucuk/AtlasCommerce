using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Interfaces
{
    public interface ICatalogDbContext
    {
        DbSet<Category> Categories { get; }
        DbSet<Brand> Brands { get; }
        DbSet<Product> Products { get; }
        DbSet<ProductImage> ProductImages { get; }
        DbSet<ProductAttribute> ProductAttributes { get; }
        DbSet<ProductVariant> ProductVariants { get; }
        DbSet<VariantAttribute> VariantAttributes { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
