namespace NexoMovil.Core.DTOs
{
    /// <summary>
    /// Datos de entrada para crear un usuario (CU-07).
    /// </summary>
    public class CreateUserDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Telephone { get; set; }
        public string Role { get; set; } = "Seller";
    }
}
