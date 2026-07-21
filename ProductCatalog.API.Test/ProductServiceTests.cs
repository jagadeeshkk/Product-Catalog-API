using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;
using ProductCatalog.API.Business;
using ProductCatalog.API.Data;
using ProductCatalog.API.Utility.Exception;
using ProductCatalog.API.Utility.Model;

namespace ProductCatalog.API.Test
{


    [TestFixture]
    public class ProductServiceTests
    {
        private ProductCatalogDbContext _context;
        private Mock<ILogger<ProductService>> _loggerMock;
        private ProductService _service;

        [SetUp]
        public void SetUp()
        {
            // Configure an isolated, fresh In-Memory database instance for every test run
            var options = new DbContextOptionsBuilder<ProductCatalogDbContext>()
                .UseSqlite("Data Source=ProductDatabase.db")
                .Options;

            _context = new ProductCatalogDbContext(options);
            _loggerMock = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_context, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetAllProductsAsync Tests

        [Test]
        public async Task GetAllProductsAsync_WhenProductsExist_ReturnsMappedOrderedList()
        {
            // Arrange
            _context.ProductDetails.AddRange(new List<ProductDetail>
        {
            new() { Id = 3, Title = "Orange", Summary = "Sweet orange", ImageUrl = "orange.png", Price = "$1.50/each" },
            new() { Id = 1, Title = "Apple", Summary = "Crisp apple", ImageUrl = "apple.png", Price = "$0.99/each" }
        });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(1)); // Confirms OrderBy(p => p.Id)
            Assert.That(result[0].Title, Is.EqualTo("Apple"));
            Assert.That(result[1].Id, Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllProductsAsync_WhenNoProducts_ReturnsEmptyList()
        {
            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region GetProductByIdAsync Tests

        [Test]
        public async Task GetProductByIdAsync_WhenProductExists_ReturnsProductDetail()
        {
            // Arrange
            var targetProduct = new ProductDetail { Id = 42, Title = "Banana", Price = "$0.50/lb" };
            _context.ProductDetails.Add(targetProduct);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetProductByIdAsync(42);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(42));
            Assert.That(result.Title, Is.EqualTo("Banana"));
        }

        [Test]
        public void GetProductByIdAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _service.GetProductByIdAsync(999));
        }

        #endregion

        #region GetProductMetricsAsync Tests

        [Test]
        public async Task GetProductMetricsAsync_WhenNoProducts_ReturnsEmptyMetricsObject()
        {
            // Act
            var result = await _service.GetProductMetricsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalProducts, Is.EqualTo(0));
            Assert.That(result.AveragePrice, Is.EqualTo(0));
        }

        [Test]
        public async Task GetProductMetricsAsync_WithValidPriceStrings_CalculatesAveragesAndExtractedUnits()
        {
            // Arrange
            _context.ProductDetails.AddRange(new List<ProductDetail>
        {
            new() { Id = 1, Title = "Apples", Price = "$3.00/lb" },
            new() { Id = 2, Title = "Berries", Price = "$5.00/lb" },
            new() { Id = 3, Title = "Melon", Price = "$4.00/each" },
            new() { Id = 4, Title = "Lettuce", Price = "$2.00/head" }
        });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetProductMetricsAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalProducts, Is.EqualTo(4));
                Assert.That(result.AveragePrice, Is.EqualTo(3.50)); // (3 + 5 + 4 + 2) / 4 = 14 / 4 = 3.5

                Assert.That(result.MostExpensiveProduct.Id, Is.EqualTo(2));
                Assert.That(result.MostExpensiveProduct.Price, Is.EqualTo("$5.00/lb"));

                Assert.That(result.LeastExpensiveProduct.Id, Is.EqualTo(4));
                Assert.That(result.LeastExpensiveProduct.Price, Is.EqualTo("$2.00/head"));

                // Verify Price Suffix Unit Groupings
                Assert.That(result.ByPriceUnitProduct.LB, Is.EqualTo(2));
                Assert.That(result.ByPriceUnitProduct.Each, Is.EqualTo(1));
                Assert.That(result.ByPriceUnitProduct.Head, Is.EqualTo(1));
                Assert.That(result.ByPriceUnitProduct.Bunch, Is.EqualTo(0));
            });
        }

        [Test]
        public async Task GetProductMetricsAsync_WithMalformedPriceField_FallsBackToZeroSafely()
        {
            // Arrange
            _context.ProductDetails.Add(new ProductDetail { Id = 10, Title = "Free Samples", Price = "BROKEN_PRICE_STRING" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetProductMetricsAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalProducts, Is.EqualTo(1));
                Assert.That(result.AveragePrice, Is.EqualTo(0.00));
                Assert.That(result.ByPriceUnitProduct.Each, Is.EqualTo(1)); // Default fallback parameter code path
            });
        }

        #endregion
    }

}
