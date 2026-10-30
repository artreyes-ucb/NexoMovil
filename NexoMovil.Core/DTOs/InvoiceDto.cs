namespace NexoMovil.Core.DTOs
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public string IssuedAt { get; set; } = null!;

        public int SaleId { get; set; }
        public string SaleDate { get; set; } = null!;

        public int ProductId { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }
        public string SellerName { get; set; } = null!;
        public string SellerEmail { get; set; } = null!;
    }
}
