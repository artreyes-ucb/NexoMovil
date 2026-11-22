using Microsoft.EntityFrameworkCore;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Infrastructure.Data;
using NexoMovil.Infrastructure.Queries;

namespace NexoMovil.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly IDapperContext _dapper;

        public UserRepository(NexoMovilContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllDapperAsync()
        {
            return await _dapper.QueryAsync<UserDto>(UserQueries.GetAll);
        }

        public async Task<UserDto?> GetByIdDapperAsync(int id)
        {
            return await _dapper.QueryFirstOrDefaultAsync<UserDto>(
                UserQueries.GetById,
                new { Id = id });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _entities.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
