using NexoMovil.Core.Entities;

namespace NexoMovil.Core.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllAvailableAsync();
    }
}
