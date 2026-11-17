namespace NexoMovil.Core.DTOs
{
    public class CashCloseReportDto
    {
        public string Date { get; set; } = null!;
        public int TotalSales { get; set; }
        public decimal TotalAmount { get; set; }
        public IEnumerable<SellerSummaryDto> Sellers { get; set; } = new List<SellerSummaryDto>();
    }
}
