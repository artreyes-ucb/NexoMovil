namespace NexoMovil.Core.Entities
{
    public class Invoice : BaseEntity
    {
        public int SaleId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime IssuedAt { get; set; }

        public virtual Sale Sale { get; set; } = null!;
    }
}
