using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoMovil.Core.Entities;

namespace NexoMovil.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("products");

            entity.Property(e => e.Model)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Brand)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Stock)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.Price)
                .IsRequired()

                // decimal(18,2) es para representar los decimales. Los dos números significan:
                // 18 = total de dígitos que tiene el número
                // 2  = Dígitos van después del punto decimal

                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}