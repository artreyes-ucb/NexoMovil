using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.QueryFilters;

namespace NexoMovil.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAvailableProductsAsync(int limit = 10);
        Task<IEnumerable<Product>> GetProductsAsync(ProductQueryFilter filter);
        Task<Product?> GetProductByIdAsync(int id);
        Task InsertProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<Product> AddInventoryIngressAsync(InventoryIngressDto dto);
    }
}
