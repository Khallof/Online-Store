using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Repository.Data.Configuration
{
    public class ProductCatalogConfig : IEntityTypeConfiguration<ProductCatalog>
    {
        public void Configure(EntityTypeBuilder<ProductCatalog> entity)
        {
            // ==================================================
            // Indexes
            // ==================================================
            entity.HasIndex(e => e.CategoryID, "IX_ProductCatalog_CategoryID");
            entity.HasIndex(e => e.Price, "IX_ProductCatalog_Price");


            entity.ToTable("ProductCatalog");

            entity.HasKey(e => e.ProductID);

            // ==================================================
            // Properties
            // ==================================================
            entity.Property(e => e.ProductName)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.Description)
                  .HasMaxLength(500);      // nullable — no IsRequired()

            entity.Property(e => e.Price)
                  .IsRequired();

            entity.Property(e => e.QuantityInStock)
                  .IsRequired();

            // ==================================================
            // Relationships
            // ==================================================
            entity.HasOne(e => e.ProductCategory)
                  .WithMany(c => c.ProductCatalog)
                  .HasForeignKey(e => e.CategoryID)
                  .HasConstraintName("FK_ProductCatalog_ProductCategory")
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}