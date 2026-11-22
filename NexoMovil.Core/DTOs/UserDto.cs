namespace NexoMovil.Core.DTOs
{
    /// <summary>
    /// Datos de un usuario expuestos por la API.
    /// Nunca incluye la contraseña ni su hash (CU-07).
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telephone { get; set; }
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
