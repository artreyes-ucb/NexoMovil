namespace NexoMovil.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Model { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}

