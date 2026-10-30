using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Infrastructure.Data;

namespace NexoMovil.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(NexoMovilContext context) : base(context) { }
    }
}
