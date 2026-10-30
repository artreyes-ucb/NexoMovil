using Microsoft.EntityFrameworkCore;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Infrastructure.Data;

namespace NexoMovil.Infrastructure.Repositories
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(NexoMovilContext context) : base(context) { }

        // RN-12: una venta solo puede tener una factura.
        public async Task<bool> ExistsBySaleIdAsync(int saleId)
        {
            return await _entities.AnyAsync(i => i.SaleId == saleId);
        }

        public async Task<Invoice?> GetByIdWithDetailsAsync(int id)
        {
            return await WithDetails()
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Invoice?> GetBySaleIdWithDetailsAsync(int saleId)
        {
            return await WithDetails()
                .FirstOrDefaultAsync(i => i.SaleId == saleId);
        }

        // CU-04: listar todas las facturas con sus datos relacionados.
        public async Task<IEnumerable<Invoice>> GetAllWithDetailsAsync()
        {
            return await WithDetails()
                .OrderBy(i => i.Id)
                .ToListAsync();
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

        private IQueryable<Invoice> WithDetails()
        {
            return _entities
                .Include(i => i.Sale)
                    .ThenInclude(s => s.Product)
                .Include(i => i.Sale)
                    .ThenInclude(s => s.User);
        }
    }
}
