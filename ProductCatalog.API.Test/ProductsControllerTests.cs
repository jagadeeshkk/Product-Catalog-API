using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;
using ProductCatalog.API.Business;
using ProductCatalog.API.Controllers;
using ProductCatalog.API.Utility.Exception;
using ProductCatalog.API.Utility.Model;

namespace ProductCatalog.API.Test
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private Mock<IProductService> _productServiceMock = null!;
        private Mock<IValidator<int>> _validatorMock = null!;
        private ProductsController _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _productServiceMock = new Mock<IProductService>();
            _validatorMock = new Mock<IValidator<int>>();

            // Default: any id passes validation unless a specific test overrides this.
            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _sut = new ProductsController(
                _productServiceMock.Object,
                Mock.Of<ILogger<ProductsController>>(),
                _validatorMock.Object);
        }

        [Test]
        public async Task GetAllProducts_ReturnsOkWithProducts()
        {
            var products = new List<Product> { new() { Id = 1, Title = "Apple" } };
            _productServiceMock.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(products);

            var result = await _sut.GetAllProducts() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.SameAs(products));
        }

        [Test]
        public async Task GetAllProducts_ServiceThrows_Returns500()
        {
            _productServiceMock.Setup(s => s.GetAllProductsAsync())
                            .ThrowsAsync(new InvalidOperationException("db unavailable"));

            var result = await _sut.GetAllProducts() as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetProductById_ValidExistingId_ReturnsOk()
        {
            var detail = new ProductDetail { Id = 3, Title = "Carrot" };
            _productServiceMock.Setup(s => s.GetProductByIdAsync(3)).ReturnsAsync(detail);

            var result = await _sut.GetProductById(3) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.SameAs(detail));
        }

        [Test]
        public async Task GetProductById_MissingId_ReturnsNotFound()
        {
            _productServiceMock.Setup(s => s.GetProductByIdAsync(99))
                            .ThrowsAsync(NotFoundException.ForProduct(99));

            var result = await _sut.GetProductById(99) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetProductMetrics_ReturnsOkWithMetrics()
        {
            var metrics = new Metrics { TotalProducts = 5 };
            _productServiceMock.Setup(s => s.GetProductMetricsAsync()).ReturnsAsync(metrics);

            var result = await _sut.GetProductMetrics() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.SameAs(metrics));
        }
    }
}
