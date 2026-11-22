using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Exceptions;
using NexoMovil.Core.Interfaces;
using NexoMovil.Services.Interfaces;
using System.Net;

namespace NexoMovil.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public UserService(IUnitOfWork unitOfWork, IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _unitOfWork.UserRepository.GetAllDapperAsync();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            return await _unitOfWork.UserRepository.GetByIdDapperAsync(id);
        }

        public async Task<User> CreateUserAsync(CreateUserDto dto)
        {
            // RN-04: el correo electrónico debe ser único.
            var existing = await _unitOfWork.UserRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new BusinessException("El email ya está registrado.", HttpStatusCode.Conflict);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = _passwordService.Hash(dto.Password),
                Telephone = dto.Telephone,
                Role = dto.Role,
                // El usuario se crea siempre activo.
                IsActive = true
            };

            await _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                throw new BusinessException("Usuario no encontrado.", HttpStatusCode.NotFound);

            // RN-04: si el correo cambia, debe seguir siendo único.
            if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _unitOfWork.UserRepository.GetByEmailAsync(dto.Email);
                if (existing != null)
                    throw new BusinessException("El email ya está registrado.", HttpStatusCode.Conflict);
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.Telephone = dto.Telephone;
            user.Role = dto.Role;

            // Si no se envía contraseña se conserva la actual.
            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.Password = _passwordService.Hash(dto.Password);

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task DeactivateUserAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                throw new BusinessException("Usuario no encontrado.", HttpStatusCode.NotFound);

            // Baja lógica: el registro no se elimina.
            user.IsActive = false;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateUserAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                throw new BusinessException("Usuario no encontrado.", HttpStatusCode.NotFound);

            if (user.IsActive)
                throw new BusinessException("El usuario ya se encuentra activo.", HttpStatusCode.BadRequest);

            user.IsActive = true;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
