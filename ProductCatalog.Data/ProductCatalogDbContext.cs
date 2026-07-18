using Microsoft.EntityFrameworkCore;
using ProductCatalog.API.Utility.Model;

namespace ProductCatalog.API.Data
{
    public class ProductCatalogDbContext: DbContext
    {
        public ProductCatalogDbContext(DbContextOptions<ProductCatalogDbContext> options) : base(options) { }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductDetail> ProductDetails => Set<ProductDetail>();
    }
}
