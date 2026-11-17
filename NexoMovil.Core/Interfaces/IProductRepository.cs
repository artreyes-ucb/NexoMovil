using NexoMovil.Core.Entities;
using NexoMovil.Core.QueryFilters;

namespace NexoMovil.Core.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<IEnumerable<Product>> GetFilteredDapperAsync(ProductQueryFilter filter);

        Task<Product?> GetByIdDapperAsync(int id);

        Task<IEnumerable<Product>> GetAllAvailableDapperAsync(int limit = 10);
    }
}
