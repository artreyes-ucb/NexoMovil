using NexoMovil.Core.Entities;

namespace NexoMovil.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task<Invoice?> GetInvoiceBySaleIdAsync(int saleId);
        Task<Invoice> GenerateInvoiceAsync(int saleId);
    }
}
