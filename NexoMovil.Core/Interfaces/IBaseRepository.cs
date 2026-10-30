using NexoMovil.Core.Entities;

namespace NexoMovil.Core.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task Add(T entity);
        void Update(T entity);
        Task Delete(int id);
        Task SaveChangesAsync();
    }
}
