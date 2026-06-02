using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Repository.Data.Configuration
{
    public class ReviewConfig : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> entity)
        {
            // ==================================================
            // Indexes
            // ==================================================
            entity.HasIndex(e => e.CustomerID, "IX_Reviews_CustomerID");
            entity.HasIndex(e => e.ProductID, "IX_Reviews_ProductID");

            // Unique — one review per customer per product
            entity.HasIndex(e => new { e.ProductID, e.CustomerID }, "UQ_Reviews_Product_Customer")
                  .IsUnique();

            entity.HasKey(e => e.ReviewID);

            // ==================================================
            // Properties
            // ==================================================
            entity.Property(e => e.Rating)
                  .IsRequired();

            entity.Property(e => e.ReviewText)
                  .HasMaxLength(500);        // nullable — no IsRequired()

            entity.Property(e => e.ReviewDate)
                  .HasDefaultValueSql("GETDATE()")
                  .IsRequired();

            // ==================================================
            // Relationships
            
            // ==================================================

            // Review belongs to Customer — NO ACTION
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Review)
                  .HasForeignKey(e => e.CustomerID)
                  .HasConstraintName("FK_Reviews_Customers")
                  .OnDelete(DeleteBehavior.NoAction);

            // Review belongs to Product — CASCADE
            entity.HasOne(e => e.ProductCatalog)
                  .WithMany(p => p.Review)
                  .HasForeignKey(e => e.ProductID)
                  .HasConstraintName("FK_Reviews_ProductCatalog")
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}