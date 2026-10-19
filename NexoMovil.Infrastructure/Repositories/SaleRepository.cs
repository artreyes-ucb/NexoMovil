using Microsoft.EntityFrameworkCore;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Infrastructure.Data;

namespace NexoMovil.Infrastructure.Repositories
{
    public class SaleRepository : BaseRepository<Sale>, ISaleRepository
    {
        public SaleRepository(NexoMovilContext context) : base(context) { }

        public async Task<Sale?> GetByIdWithDetailsAsync(int id)
        {
            return await _entities
                .Include(s => s.Product)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Sale>> GetAllWithDetailsAsync()
        {
            return await _entities
                .Include(s => s.Product)
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<bool> ExistsByProductIdAsync(int productId)
        {
            return await _entities.AnyAsync(s => s.ProductId == productId);
        }
    }
}