using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Exceptions;
using NexoMovil.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace NexoMovil.Api.Controllers
{
    /// <summary>
    /// CU-08: Iniciar sesión. Endpoint público que valida las credenciales
    /// (PBKDF2) y devuelve un token JWT firmado con HS256.
    /// </summary>
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        private readonly IPasswordService _passwordService;

        public TokenController(
            IConfiguration configuration,
            ISecurityService securityService,
            IPasswordService passwordService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _passwordService = passwordService;
        }

        // POST: api/token
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var (isValid, user) = await IsValidUser(dto);

            // 2a. Credenciales incorrectas: no se genera token.
            if (!isValid || user == null)
                throw new BusinessException("Credenciales inválidas.", HttpStatusCode.Unauthorized);

            var token = GenerateToken(user);
            return Ok(new { token });
        }

        private async Task<(bool, User?)> IsValidUser(LoginRequestDto login)
        {
            var user = await _securityService.GetLoginByCredentialsAsync(login);

            if (user == null)
                return (false, null);

            // RN-07: un usuario inactivo tiene prohibido iniciar sesión.
            if (!user.IsActive)
                throw new BusinessException("Usuario inactivo, contacte al administrador.", HttpStatusCode.Forbidden);

            var isValid = _passwordService.Check(user.Password, login.Password);
            return (isValid, user);
        }

        private string GenerateToken(User user)
        {
            // HEADER: clave simétrica + algoritmo de firma.
            var symmetricKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]!));
            var signingCredentials = new SigningCredentials(
                symmetricKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);

            // PAYLOAD: claims del usuario (sin datos sensibles).
            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Name", $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // El documento fija una vigencia de 8 horas.
            var expirationHours = _configuration.GetValue<int?>("Authentication:ExpirationHours") ?? 8;

            var payload = new JwtPayload(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(expirationHours));

            // FIRMA: header + payload firmados.
            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
