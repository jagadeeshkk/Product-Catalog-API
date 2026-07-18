using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductCatalog.API.Utility;
using ProductCatalog.API.Utility.Model;
using System.Text.Json;

namespace ProductCatalog.API.Data
{
    public static class DBSeeder
    {
        public static async Task SeedAsync(ProductCatalogDbContext context, ILogger logger)
        {
            await SeedProductsAsync(context, logger);
            await SeedProductDetailsAsync(context, logger);
        }
        private static async Task SeedProductsAsync(ProductCatalogDbContext context, ILogger logger)
        {
            if (await context.Products.AsNoTracking().AnyAsync())
            {
                return;
            }
            var seedPath = Path.Combine(AppContext.BaseDirectory,"Seed", "products.json");
            if (!File.Exists(seedPath))
            {
                logger.LogWarning("Seed file not found at {Path}; skipping database seed.", seedPath);
                return;
            }

            var json = await File.ReadAllTextAsync(seedPath);
            var rawRecords = JsonSerializer.Deserialize<List<Product>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();


            var products = new List<Product>();
            foreach (var raw in rawRecords)
            {
                try
                {
                    products.Add(new Product
                    {
                        Id = raw.Id,
                        Title = raw.Title,
                        Summary = raw.Summary,
                        ImageUrl = raw.ImageUrl
                    });
                }
                catch (FormatException ex)
                {
                    logger.LogError(ex, "Skipping seed record {Id} ({Title})",
                        raw.Id, raw.Title);
                }
            }
            context.ChangeTracker.Clear();
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} products from {Path}", products.Count, seedPath);
        }

        private static async Task SeedProductDetailsAsync(ProductCatalogDbContext context, ILogger logger)
        {
            if (await context.ProductDetails.AsNoTracking().AnyAsync())
            {
                return;
            }
            var seedPath = Path.Combine(AppContext.BaseDirectory, "Seed", "productdetails.json");
            if (!File.Exists(seedPath))
            {
                logger.LogWarning("Seed file not found at {Path}; skipping database seed.", seedPath);
                return;
            }

            var json = await File.ReadAllTextAsync(seedPath);
            var rawRecords = JsonSerializer.Deserialize<List<ProductDetail>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var productdetails = new List<ProductDetail>();
            foreach (var raw in rawRecords)
            {
                try
                {
                    //var(amount, unit) = PriceFormat.Parse(raw.Price);
                    productdetails.Add(new ProductDetail
                    {
                        Id = raw.Id,
                        Title = raw.Title,
                        Summary = raw.Summary,
                        ImageUrl = raw.ImageUrl,
                        Description = raw.Description,
                        Price = raw.Price
                    });
                }
                catch (FormatException ex)
                {
                    logger.LogError(ex, "Skipping seed record {Id} ({Title})",
                        raw.Id, raw.Title);
                }
            }
            context.ChangeTracker.Clear();
            context.ProductDetails.AddRange(productdetails);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} products from {Path}", productdetails.Count, seedPath);
        }
    }
}
