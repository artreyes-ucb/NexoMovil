using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoMovil.Core.Entities;

namespace NexoMovil.Infrastructure.Data.Configurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> entity)
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("sales");

            entity.HasIndex(e => e.ProductId, "FK_Sale_Product");
            entity.HasIndex(e => e.UserId, "FK_Sale_User");

            entity.Property(e => e.Date)
                .IsRequired()
                .HasColumnType("datetime");

            entity.Property(e => e.IsActive)
                .HasColumnType("bit")
                .IsRequired();

            entity.Property(e => e.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            entity.HasOne(d => d.Product)
                .WithMany(p => p.Sales)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sale_Product");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Sales)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sale_User");
        }
    }
}