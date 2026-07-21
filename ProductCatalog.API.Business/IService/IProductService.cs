using ProductCatalog.API.Utility.Model;

namespace ProductCatalog.API.Business
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<ProductDetail> GetProductByIdAsync(int id);
        Task<Metrics> GetProductMetricsAsync();
    }
}
