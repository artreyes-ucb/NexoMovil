using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Exceptions;
using NexoMovil.Core.Interfaces;
using NexoMovil.Services.Interfaces;
using System.Net;

namespace NexoMovil.Services.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // CU-04: obtener una factura por su Id.
        public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id)
        {
            return await _unitOfWork.InvoiceRepository.GetByIdDapperAsync(id);
        }

        // CU-04: obtener la factura asociada a una venta.
        public async Task<InvoiceDto?> GetInvoiceBySaleIdAsync(int saleId)
        {
            return await _unitOfWork.InvoiceRepository.GetBySaleIdDapperAsync(saleId);
        }

        // CU-04: generar la factura de una venta.
        public async Task<InvoiceDto> GenerateInvoiceAsync(int saleId)
        {
            var sale = await _unitOfWork.SaleRepository.GetByIdAsync(saleId);
            if (sale == null)
                throw new BusinessException("Venta no encontrada.", HttpStatusCode.NotFound);

            // RN-05: una venta anulada no puede facturarse.
            if (!sale.IsActive)
                throw new BusinessException("No se puede generar una factura para una venta anulada.", HttpStatusCode.Conflict);

            // RN-12: una venta solo puede tener una factura.
            if (await _unitOfWork.InvoiceRepository.ExistsBySaleIdAsync(saleId))
                throw new BusinessException("Esta venta ya tiene una factura generada.", HttpStatusCode.Conflict);

            var invoice = new Invoice
            {
                SaleId = saleId,
                IssuedAt = DateTime.Now,
                // RN-13: número único con formato PH-2026-000001.
                InvoiceNumber = await _unitOfWork.InvoiceRepository.GenerateInvoiceNumberAsync()
            };

            await _unitOfWork.InvoiceRepository.Add(invoice);
            await _unitOfWork.SaveChangesAsync();

            var createdInvoice = await _unitOfWork.InvoiceRepository.GetByIdDapperAsync(invoice.Id);
            if (createdInvoice == null)
                throw new BusinessException("No se pudo recuperar la factura generada.", HttpStatusCode.InternalServerError);

            return createdInvoice;
        }
    }
}
