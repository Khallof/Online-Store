using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Repository.Data.Configuration
{
    public class PaymentConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> entity)
        {
            // ==================================================
            // Indexes
            // ==================================================
            entity.HasIndex(e => e.OrderID, "IX_Payments_OrderID");

            // Unique — enforces 1-to-1 relationship with Orders
            entity.HasIndex(e => e.OrderID, "UQ_Payments_OrderID")
                  .IsUnique();

            entity.HasIndex(e => e.TransactionDate, "IX_Payments_TransactionDate");

            entity.HasKey(e => e.PaymentID);

            // ==================================================
            // Properties
            // ==================================================
            entity.Property(e => e.Amount)
                  .IsRequired();

            entity.Property(e => e.PaymentMethod)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(e => e.TransactionDate)
                  .HasDefaultValueSql("GETDATE()")
                  .IsRequired();

            // ==================================================
            // Relationships
            // ==================================================
            entity.HasOne(e => e.Order)
                  .WithOne(o => o.Payment)
                  .HasForeignKey<Payment>(e => e.OrderID)
                  .HasConstraintName("FK_Payments_Orders")
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}