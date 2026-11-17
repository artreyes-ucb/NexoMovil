namespace NexoMovil.Core.DTOs
{
    public class SellerSummaryDto
    {
        public int UserId { get; set; }
        public string SellerName { get; set; } = null!;
        public string SellerEmail { get; set; } = null!;
        public int SalesCount { get; set; }
        public decimal SubTotal { get; set; }
    }
}
