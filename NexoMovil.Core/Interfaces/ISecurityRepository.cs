using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;

namespace NexoMovil.Core.Interfaces
{
    /// <summary>
    /// Repositorio de seguridad: obtiene las credenciales del usuario
    /// para el proceso de autenticación (CU-08).
    /// </summary>
    public interface ISecurityRepository : IBaseRepository<User>
    {
        Task<User?> GetLoginByCredentialsAsync(LoginRequestDto login);
    }
}
