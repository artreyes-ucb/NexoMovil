namespace NexoMovil.Core.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? RegistrationDate { get; set; }
    }
}
