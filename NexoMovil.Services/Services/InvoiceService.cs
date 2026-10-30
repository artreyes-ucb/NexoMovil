using NexoMovil.Core.Entities;
using NexoMovil.Core.Exceptions;
using NexoMovil.Core.Interfaces;
using NexoMovil.Services.Interfaces;
using System.Net;

namespace NexoMovil.Services.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ISaleRepository _saleRepository;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            ISaleRepository saleRepository)
        {
            _invoiceRepository = invoiceRepository;
            _saleRepository = saleRepository;
        }

        // CU-04: listar todas las facturas.
        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await _invoiceRepository.GetAllWithDetailsAsync();
        }

        // CU-04: obtener una factura por su Id.
        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            return await _invoiceRepository.GetByIdWithDetailsAsync(id);
        }

        // CU-04: obtener la factura asociada a una venta.
        public async Task<Invoice?> GetInvoiceBySaleIdAsync(int saleId)
        {
            return await _invoiceRepository.GetBySaleIdWithDetailsAsync(saleId);
        }

        // CU-04: generar la factura de una venta.
        public async Task<Invoice> GenerateInvoiceAsync(int saleId)
        {
            var sale = await _saleRepository.GetByIdAsync(saleId);
            if (sale == null)
                throw new BusinessException("Venta no encontrada.", HttpStatusCode.NotFound);

            // RN-05: una venta anulada no puede facturarse.
            if (!sale.IsActive)
                throw new BusinessException("No se puede generar una factura para una venta anulada.", HttpStatusCode.Conflict);

            // RN-12: una venta solo puede tener una factura.
            if (await _invoiceRepository.ExistsBySaleIdAsync(saleId))
                throw new BusinessException("Esta venta ya tiene una factura generada.", HttpStatusCode.Conflict);

            var invoice = new Invoice
            {
                SaleId = saleId,
                IssuedAt = DateTime.Now,
                // RN-13: número único con formato PH-2026-000001.
                InvoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync()
            };

            await _invoiceRepository.Add(invoice);
            await _invoiceRepository.SaveChangesAsync();

            var createdInvoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoice.Id);
            if (createdInvoice == null)
                throw new BusinessException("No se pudo recuperar la factura generada.", HttpStatusCode.InternalServerError);

            return createdInvoice;
        }
    }
}
