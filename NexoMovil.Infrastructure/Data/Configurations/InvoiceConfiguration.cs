using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoMovil.Core.Entities;

namespace NexoMovil.Infrastructure.Data.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> entity)
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("invoices");

            // RN-12: una venta solo puede tener una factura.
            entity.HasIndex(e => e.SaleId)
                .IsUnique()
                .HasDatabaseName("UQ_Invoice_SaleId");

            // RN-13: el número de factura es único.
            entity.HasIndex(e => e.InvoiceNumber)
                .IsUnique()
                .HasDatabaseName("UQ_Invoice_Number");

            entity.Property(e => e.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.IssuedAt)
                .IsRequired()
                .HasColumnType("datetime");

            entity.HasOne(e => e.Sale)
                .WithOne(s => s.Invoice)
                .HasForeignKey<Invoice>(e => e.SaleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Invoice_Sale");
        }
    }
}
