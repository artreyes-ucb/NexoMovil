using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Services.Interfaces;

namespace NexoMovil.Services.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SecurityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetLoginByCredentialsAsync(LoginRequestDto login)
        {
            return await _unitOfWork.SecurityRepository.GetLoginByCredentialsAsync(login);
        }

        public async Task RegisterUserAsync(User user)
        {
            await _unitOfWork.SecurityRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
