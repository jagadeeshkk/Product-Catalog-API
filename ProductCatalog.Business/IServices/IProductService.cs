using ProductCatalog.API.Utility.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.API.Business.IRepository
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<ProductDetail> GetProductByIdAsync(int id);
        Task<Metrics> GetProductMetricsAsync();
    }
}
