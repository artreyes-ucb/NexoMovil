using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Core.QueryFilters;
using NexoMovil.Infrastructure.Data;
using NexoMovil.Infrastructure.Queries;

namespace NexoMovil.Infrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly IDapperContext _dapper;

        public ProductRepository(NexoMovilContext context, IDapperContext dapper)
            : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Product>> GetFilteredDapperAsync(ProductQueryFilter filter)
        {
            return await _dapper.QueryAsync<Product>(
                ProductQueries.GetFiltered,
                new
                {
                    Brand = string.IsNullOrWhiteSpace(filter.Brand) ? null : filter.Brand.Trim(),
                    Model = string.IsNullOrWhiteSpace(filter.Model) ? null : filter.Model.Trim(),
                    MaxPrice = filter.MaxPrice,
                    OnlyAvailable = filter.OnlyAvailable ?? true
                });
        }

        public async Task<Product?> GetByIdDapperAsync(int id)
        {
            return await _dapper.QueryFirstOrDefaultAsync<Product>(
                ProductQueries.GetById,
                new { Id = id });
        }

        public async Task<IEnumerable<Product>> GetAllAvailableDapperAsync(int limit = 10)
        {
            return await _dapper.QueryAsync<Product>(
                ProductQueries.GetAllAvailable,
                new { Limit = limit });
        }
    }
}
