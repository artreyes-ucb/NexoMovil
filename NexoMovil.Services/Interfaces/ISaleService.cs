using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.QueryFilters;

namespace NexoMovil.Services.Interfaces
{
    public interface ISaleService
    {
        Task<IEnumerable<SaleResponseDto>> GetAllSalesDapperAsync(int limit = 10);
        Task<IEnumerable<SaleResponseDto>> GetSalesAsync(SaleQueryFilter filter);
        Task<SaleResponseDto?> GetSaleByIdAsync(int id);
        Task<Sale> InsertSaleAsync(Sale sale);
        Task UpdateSaleAsync(Sale sale);
        Task<CashCloseReportDto> GetDailyCashCloseReportAsync(string? date);
        Task AnnulSaleAsync(int id);
    }
}
