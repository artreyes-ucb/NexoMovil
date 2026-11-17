using NexoMovil.Core.DTOs;

namespace NexoMovil.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceDto?> GetInvoiceByIdAsync(int id);
        Task<InvoiceDto?> GetInvoiceBySaleIdAsync(int saleId);
        Task<InvoiceDto> GenerateInvoiceAsync(int saleId);
    }
}
