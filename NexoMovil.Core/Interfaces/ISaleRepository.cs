using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.QueryFilters;

namespace NexoMovil.Core.Interfaces
{
    public interface ISaleRepository : IBaseRepository<Sale>
    {
        Task<IEnumerable<SellerSummaryDto>> GetDailyReportAsync(DateTime date);

        Task<IEnumerable<SaleResponseDto>> GetAllWithDetailsDapperAsync(int limit = 10);

        Task<IEnumerable<SaleResponseDto>> GetFilteredWithDetailsDapperAsync(SaleQueryFilter filter);

        Task<SaleResponseDto?> GetByIdWithDetailsDapperAsync(int id);

        /// <summary>
        /// Indica si un producto tiene ventas asociadas, para impedir su eliminación física.
        /// </summary>
        Task<bool> ExistsByProductIdAsync(int productId);
    }
}
