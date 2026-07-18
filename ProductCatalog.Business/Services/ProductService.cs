using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductCatalog.API.Business.IRepository;
using ProductCatalog.API.Data;
using ProductCatalog.API.Utility.Exception;
using ProductCatalog.API.Utility.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.API.Business.Repository
{
    public class ProductService : IProductService
    {
        private readonly ProductCatalogDbContext _context;
        private readonly ILogger<ProductService> _logger;
        public ProductService(ProductCatalogDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.ProductDetails
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Select(p => new Product
                {
                    Id = p.Id,
                    Title = p.Title,
                    Summary = p.Summary,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();
        }

        public async Task<ProductDetail> GetProductByIdAsync(int id)
        {
            var product = await _context.ProductDetails.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            return product ?? throw NotFoundException.ForProduct(id);
        }

        public async Task<Metrics> GetProductMetricsAsync()
        {
            var dbProducts = await _context.ProductDetails
        .AsNoTracking()
        .Select(p => new { p.Id, p.Title, p.Price })
        .ToListAsync();

            if (!dbProducts.Any())
            {
                Metrics metrics = new Metrics();
                return metrics;
            }

            var parsedProducts = dbProducts.Select(p =>
            {
                decimal numericPrice = 0m;
                string unit = "/each"; // Default fallback banner

                if (!string.IsNullOrWhiteSpace(p.Price))
                {
                    // Extract everything before the '/' and strip the '$' symbol
                    var parts = p.Price.Split('/');
                    var numericPart = parts[0].Replace("$", "").Trim();

                    decimal.TryParse(numericPart, out numericPrice);

                    if (parts.Length > 1)
                    {
                        unit = "/" + parts[1].Trim(); // Reconstruct the unit tag (e.g., "/lb")
                    }
                }

                return new
                {
                    Source = p,
                    NumericPrice = numericPrice,
                    Unit = unit
                };
            }).ToList();

            // 3. Aggregate statistics using LINQ
            var totalProducts = parsedProducts.Count;
            var averagePrice = Math.Round(parsedProducts.Average(p => p.NumericPrice), 2);

            var mostExpensiveItem = parsedProducts.OrderByDescending(p => p.NumericPrice).First();
            var leastExpensiveItem = parsedProducts.OrderBy(p => p.NumericPrice).First();

            // Group and count occurrences of each distinct string unit suffix
            var byPriceUnit = parsedProducts
                .GroupBy(p => p.Unit)
                .ToDictionary(g => g.Key, g => g.Count());

            // 4. Construct the precise payload matching your schema
            var analytics = new Metrics
            {
                TotalProducts = totalProducts,
                AveragePrice = averagePrice,
                MostExpensiveProduct = new MostExpensive
                {
                    Id = mostExpensiveItem.Source.Id,
                    Title = mostExpensiveItem.Source.Title,
                    Price = mostExpensiveItem.Source.Price
                },
                LeastExpensiveProduct = new LeastExpensive
                {
                    Id = leastExpensiveItem.Source.Id,
                    Title = leastExpensiveItem.Source.Title,
                    Price = leastExpensiveItem.Source.Price
                },
                ByPriceUnitProduct = new ByPriceUnit
                {
                    LB = byPriceUnit.Where(x => x.Key.Equals("/lb")).Select(x => x.Value).FirstOrDefault(),
                    Each = byPriceUnit.Where(x => x.Key.Equals("/each")).Select(x => x.Value).FirstOrDefault(),
                    Bunch = byPriceUnit.Where(x => x.Key.Equals("/bunch")).Select(x => x.Value).FirstOrDefault(),
                    Head = byPriceUnit.Where(x => x.Key.Equals("/head")).Select(x => x.Value).FirstOrDefault(),
                    Bag = byPriceUnit.Where(x => x.Key.Equals("/bag")).Select(x => x.Value).FirstOrDefault()
                }
            };

            return analytics;
        }
    }
}
