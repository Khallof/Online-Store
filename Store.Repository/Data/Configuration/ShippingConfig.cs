using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Repository.Data.Configuration
{
    public class ShippingConfig : IEntityTypeConfiguration<Shipping>
    {
        public void Configure(EntityTypeBuilder<Shipping> entity)
        {
            // ==================================================
            // Indexes
            // ==================================================
            entity.HasIndex(e => e.OrderID, "IX_Shippings_OrderID");
            entity.HasIndex(e => e.ShippingStatus, "IX_Shippings_ShippingStatus");
            entity.HasIndex(e => e.TrackingNumber, "IX_Shippings_TrackingNumber");

            // Unique — enforces 1-to-1 relationship with Orders
            entity.HasIndex(e => e.OrderID, "UQ_Shippings_OrderID")
                  .IsUnique();

            entity.HasKey(e => e.ShippingID);

            // ==================================================
            // Properties
            // ==================================================
            entity.Property(e => e.CarrierName)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.TrackingNumber)
                  .HasMaxLength(50);         // nullable — no IsRequired()

            entity.Property(e => e.ShippingStatus)
                  .HasDefaultValue(0)
                  .IsRequired();

            entity.Property(e => e.EstimatedDeliveryDate); // nullable
            entity.Property(e => e.ActualDeliveryDate);    // nullable

            // ==================================================
            // Relationships
            // ==================================================
            entity.HasOne(s => s.Order)
                  .WithOne(o => o.Shipping)
                  .HasForeignKey<Shipping>(s => s.OrderID)
                  .HasConstraintName("FK_Shippings_Orders")
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}