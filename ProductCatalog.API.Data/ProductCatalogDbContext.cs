using Microsoft.EntityFrameworkCore;
using ProductCatalog.API.Utility.Model;
using System.Diagnostics.CodeAnalysis;

namespace ProductCatalog.API.Data
{
    [ExcludeFromCodeCoverage]
    public class ProductCatalogDbContext: DbContext
    {
        public ProductCatalogDbContext(DbContextOptions<ProductCatalogDbContext> options) : base(options) { }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductDetail> ProductDetails => Set<ProductDetail>();
    }
}
