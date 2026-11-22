using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;

namespace NexoMovil.Services.Interfaces
{
    public interface ISecurityService
    {
        Task<User?> GetLoginByCredentialsAsync(LoginRequestDto login);
        Task RegisterUserAsync(User user);
    }
}
