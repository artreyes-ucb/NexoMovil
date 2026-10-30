using NexoMovil.Core.Entities;

namespace NexoMovil.Core.Interfaces
{
    /// <summary>
    /// Repositorio de la entidad relacionada User (vendedor).
    /// Hereda GetByIdAsync(int id) de IBaseRepository.
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
    }
}
