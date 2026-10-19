using Microsoft.EntityFrameworkCore;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Infrastructure.Data;

namespace NexoMovil.Infrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(NexoMovilContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetAllAvailableAsync()
        {
            return await _entities
                .Where(p => p.Stock > 0)
                .ToListAsync();
        }
    }
}