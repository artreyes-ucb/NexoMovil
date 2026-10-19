namespace NexoMovil.Core.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Telephone { get; set; }
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }

        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
