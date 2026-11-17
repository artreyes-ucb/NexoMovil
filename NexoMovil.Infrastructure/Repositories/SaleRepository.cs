using Microsoft.EntityFrameworkCore;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Core.QueryFilters;
using NexoMovil.Infrastructure.Data;
using NexoMovil.Infrastructure.Queries;

namespace NexoMovil.Infrastructure.Repositories
{
    public class SaleRepository : BaseRepository<Sale>, ISaleRepository
    {
        private readonly IDapperContext _dapper;

        public SaleRepository(NexoMovilContext context, IDapperContext dapper)
            : base(context)
        {
            _dapper = dapper;
        }

        // RN-10: el reporte solo considera ventas activas.
        public async Task<IEnumerable<SellerSummaryDto>> GetDailyReportAsync(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return await _dapper.QueryAsync<SellerSummaryDto>(
                SaleQueries.GetDailyReport,
                new { Start = start, End = end });
        }

        public async Task<IEnumerable<SaleResponseDto>> GetAllWithDetailsDapperAsync(int limit = 10)
        {
            return await _dapper.QueryAsync<SaleResponseDto>(
                SaleQueries.GetAllWithDetails,
                new { Limit = limit });
        }

        public async Task<IEnumerable<SaleResponseDto>> GetFilteredWithDetailsDapperAsync(SaleQueryFilter filter)
        {
            return await _dapper.QueryAsync<SaleResponseDto>(
                SaleQueries.GetFilteredWithDetails,
                new
                {
                    IsActive = filter.IsActive,
                    UserId = filter.UserId,
                    ProductId = filter.ProductId
                });
        }

        public async Task<SaleResponseDto?> GetByIdWithDetailsDapperAsync(int id)
        {
            return await _dapper.QueryFirstOrDefaultAsync<SaleResponseDto>(
                SaleQueries.GetByIdWithDetails,
                new { Id = id });
        }

        public async Task<bool> ExistsByProductIdAsync(int productId)
        {
            return await _entities.AnyAsync(s => s.ProductId == productId);
        }
    }
}
