using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Repository.Data.Configuration
{
    public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> entity)
        {
            // ==================================================
            // Indexes
            // ==================================================
            entity.HasIndex(e => e.OrderID, "IX_OrderItems_OrderID");
            entity.HasIndex(e => e.ProductID, "IX_OrderItems_ProductID");

            // Unique — same product can't appear twice in same order
            entity.HasIndex(e => new { e.OrderID, e.ProductID }, "UQ_OrderItems_OrderID_ProductID")
                  .IsUnique();

            entity.HasKey(e => e.OrderItemID);

            // ==================================================
            // Properties
            // ==================================================
            entity.Property(e => e.Price)
                  .IsRequired();

            entity.Property(e => e.TotalItemsPrice)
                  .IsRequired();

            entity.Property(e => e.Quantity)
                  .IsRequired();

            // ==================================================
            // Relationships
            // ==================================================
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.OrderItem)
                  .HasForeignKey(e => e.OrderID)
                  .HasConstraintName("FK_OrderItems_Orders")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ProductCatalog)
                  .WithMany(p => p.OrderItem)
                  .HasForeignKey(e => e.ProductID)
                  .HasConstraintName("FK_OrderItems_ProductCatalog")
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}