using NexoMovil.Core.Entities;

namespace NexoMovil.Core.Interfaces
{
    public interface IInvoiceRepository : IBaseRepository<Invoice>
    {
        Task<bool> ExistsBySaleIdAsync(int saleId);

        Task<Invoice?> GetByIdWithDetailsAsync(int id);

        Task<Invoice?> GetBySaleIdWithDetailsAsync(int saleId);

        Task<IEnumerable<Invoice>> GetAllWithDetailsAsync();

        Task<string> GenerateInvoiceNumberAsync();
    }
}
