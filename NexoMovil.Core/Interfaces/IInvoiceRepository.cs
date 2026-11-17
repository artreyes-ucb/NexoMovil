using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;

namespace NexoMovil.Core.Interfaces
{
    public interface IInvoiceRepository : IBaseRepository<Invoice>
    {
        Task<bool> ExistsBySaleIdAsync(int saleId);

        Task<string> GenerateInvoiceNumberAsync();

        Task<InvoiceDto?> GetByIdDapperAsync(int id);

        Task<InvoiceDto?> GetBySaleIdDapperAsync(int saleId);
    }
}
