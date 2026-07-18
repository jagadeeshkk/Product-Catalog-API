using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.API.Business.IRepository;
using ProductCatalog.API.Utility.Exception;
using ProductCatalog.API.Utility.Model;

namespace ProductCatalog.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class productsController : ControllerBase
    {
        private readonly ILogger<productsController> _logger;
        private readonly IProductService _productService;
        public productsController(IProductService productService, ILogger<productsController> logger)
        {
            _logger = logger;
            _productService = productService;
        }
        // GET /api/products
        [HttpGet]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET /api/products/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var products = await _productService.GetProductByIdAsync(id);
                return Ok(products);
            }
            catch(NotFoundException ex)
            {
                _logger.LogWarning(ex, "Product with ID {Id} not found.", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products.");
                return StatusCode(500, "Internal server error");
            }
        }
        // GET /api/products/metrics
        [HttpGet("metrics")]
        [ProducesResponseType(typeof(Metrics), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductMetrics()
        {
            try
            {
                var products = await _productService.GetProductMetricsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
