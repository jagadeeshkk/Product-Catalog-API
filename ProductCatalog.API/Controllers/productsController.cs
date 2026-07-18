using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.API.Business.IRepository;

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
        [HttpGet("api/products")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("api/products/{id}")]
        public async Task<IActionResult> GetProductsbyId(int id)
        {
            try
            {
                var products = await _productService.GetProductbyID(id);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("api/products/metrics")]
        public async Task<IActionResult> GetProductMetrics()
        {
            try
            {
                var products = await _productService.GetProductAnalytics();
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
