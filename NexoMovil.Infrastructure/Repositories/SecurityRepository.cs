using Microsoft.EntityFrameworkCore;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Infrastructure.Data;

namespace NexoMovil.Infrastructure.Repositories
{
    /// <summary>
    /// Busca al usuario por su login (email). La contraseña se valida
    /// después con IPasswordService, tal como indica el documento.
    /// </summary>
    public class SecurityRepository : BaseRepository<User>, ISecurityRepository
    {
        public SecurityRepository(NexoMovilContext context) : base(context)
        {
        }

        public async Task<User?> GetLoginByCredentialsAsync(LoginRequestDto login)
        {
            return await _entities.FirstOrDefaultAsync(x => x.Email == login.Email);
        }
    }
}
