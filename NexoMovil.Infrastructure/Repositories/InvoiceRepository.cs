using Microsoft.EntityFrameworkCore;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Infrastructure.Data;
using NexoMovil.Infrastructure.Queries;

namespace NexoMovil.Infrastructure.Repositories
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        private readonly IDapperContext _dapper;

        public InvoiceRepository(NexoMovilContext context, IDapperContext dapper)
            : base(context)
        {
            _dapper = dapper;
        }

        // RN-12: una venta solo puede tener una factura.
        public async Task<bool> ExistsBySaleIdAsync(int saleId)
        {
            return await _entities.AnyAsync(i => i.SaleId == saleId);
        }

        // RN-13: número único con formato institucional PH-2026-000001.
        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var year = DateTime.Now.Year;
            var prefix = $"PH-{year}-";

            var lastNumber = await _entities
                .Where(i => i.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNumber)
                .Select(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            var next = 1;

            if (!string.IsNullOrEmpty(lastNumber) &&
                lastNumber.Length >= 6 &&
                int.TryParse(lastNumber[^6..], out var sequence))
            {
                next = sequence + 1;
            }

            return $"{prefix}{next:D6}";
        }

        public async Task<InvoiceDto?> GetByIdDapperAsync(int id)
        {
            return await _dapper.QueryFirstOrDefaultAsync<InvoiceDto>(
                InvoiceQueries.GetById,
                new { Id = id });
        }

        public async Task<InvoiceDto?> GetBySaleIdDapperAsync(int saleId)
        {
            return await _dapper.QueryFirstOrDefaultAsync<InvoiceDto>(
                InvoiceQueries.GetBySaleId,
                new { SaleId = saleId });
        }
    }
}
