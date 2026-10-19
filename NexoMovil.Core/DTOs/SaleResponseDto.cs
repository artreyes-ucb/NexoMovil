namespace NexoMovil.Core.DTOs
{
    public class SaleResponseDto
    {
        public int SaleId { get; set; }
        public string Date { get; set; } = null!;

        public int ProductId { get; set; }

        public string Model { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public decimal UnitPrice { get; set; }

        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }

        public int UserId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
    }
}
