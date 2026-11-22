using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;

namespace NexoMovil.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<User> CreateUserAsync(CreateUserDto dto);
        Task<User> UpdateUserAsync(int id, UpdateUserDto dto);
        Task DeactivateUserAsync(int id);
        Task ActivateUserAsync(int id);
    }
}
