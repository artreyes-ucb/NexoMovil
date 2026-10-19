using Microsoft.EntityFrameworkCore;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Infrastructure.Data;

namespace NexoMovil.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly NexoMovilContext _context;
        protected readonly DbSet<T> _entities;

        public BaseRepository(NexoMovilContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _entities.ToListAsync();
        }

        public async Task<T?> GetById(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task Add(T entity)
        {
            await _entities.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _entities.Update(entity);
        }

        public async Task Delete(int id)
        {
            T? entity = await GetById(id);
            _entities.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}