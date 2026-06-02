using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Repository.Data.Configuration
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
           

            entity.HasIndex(e => e.CustomerID, "IX_Orders_CustomerID");
            entity.HasIndex(e => e.Status, "IX_Orders_Status");
            entity.HasIndex(e => e.Orderdate, "IX_Orders_OrderDate");

            entity.HasKey(e => e.OrderID);

            entity.Property(e => e.TotalAmount)
                  .IsRequired();

            entity.Property(e => e.Status)
                  .HasDefaultValue(0)
                  .IsRequired();

            entity.Property(e => e.Orderdate)
                  .HasDefaultValueSql("GETDATE()")
                  .IsRequired();

          
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Order)
                  .HasForeignKey(e => e.CustomerID)
                  .HasConstraintName("FK_Orders_Customers")
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}