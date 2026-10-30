using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;

namespace NexoMovil.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetAvailableProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task InsertProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<Product> AddInventoryIngressAsync(InventoryIngressDto dto);
    }
}
