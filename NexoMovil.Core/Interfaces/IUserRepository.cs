using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;

namespace NexoMovil.Core.Interfaces
{
    /// <summary>
    /// Repositorio de la entidad relacionada User (vendedor / administrador).
    /// Hereda GetByIdAsync(int id) de IBaseRepository.
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<IEnumerable<UserDto>> GetAllDapperAsync();
        Task<UserDto?> GetByIdDapperAsync(int id);
        Task<User?> GetByEmailAsync(string email);
    }
}
