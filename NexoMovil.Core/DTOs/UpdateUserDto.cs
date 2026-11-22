namespace NexoMovil.Core.DTOs
{
    /// <summary>
    /// Datos de entrada para actualizar un usuario (CU-07).
    /// Si Password llega vacío o nulo se conserva la contraseña actual.
    /// </summary>
    public class UpdateUserDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public string? Telephone { get; set; }
        public string Role { get; set; } = "Seller";
    }
}
