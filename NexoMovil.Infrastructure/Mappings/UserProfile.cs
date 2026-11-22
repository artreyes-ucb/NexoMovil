using AutoMapper;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;

namespace NexoMovil.Infrastructure.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // La contraseña nunca se expone: UserDto no tiene esa propiedad.
            CreateMap<User, UserDto>();
        }
    }
}
