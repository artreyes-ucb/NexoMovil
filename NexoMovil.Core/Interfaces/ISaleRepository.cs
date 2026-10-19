using NexoMovil.Core.Entities;

namespace NexoMovil.Core.Interfaces
{
    public interface ISaleRepository : IBaseRepository<Sale>
    {
        Task<Sale?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<Sale>> GetAllWithDetailsAsync();

        /// <summary>
        /// Indica si un producto tiene ventas asociadas, para impedir su eliminación física.
        /// </summary>
        Task<bool> ExistsByProductIdAsync(int productId);
    }
}
