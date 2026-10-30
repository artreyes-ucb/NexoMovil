namespace NexoMovil.Core.Entities
{
    public class Sale : BaseEntity
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Invoice? Invoice { get; set; }
    }
}
