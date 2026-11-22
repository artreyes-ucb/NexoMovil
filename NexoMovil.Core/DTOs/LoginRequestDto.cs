namespace NexoMovil.Core.DTOs
{
    /// <summary>
    /// Credenciales enviadas a POST /api/Token (CU-08).
    /// </summary>
    public class LoginRequestDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
