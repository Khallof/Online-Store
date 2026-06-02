using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Repository.Data.Configuration
{
    public class ProductImageConfig : IEntityTypeConfiguration<ProductImages>
    {
        public void Configure(EntityTypeBuilder<ProductImages> entity)
        {
            // ==================================================
            // Indexes
            // ==================================================
            entity.HasIndex(e => e.ProductID, "IX_ProductImages_ProductID");

            // Unique — no duplicate image position per product
            entity.HasIndex(e => new { e.ProductID, e.ImageOrder }, "UQ_ProductImages_Product_Order")
                  .IsUnique();

            entity.HasKey(e => e.ImageID);

            // ==================================================
            // Properties
            // ==================================================
            entity.Property(e => e.ImageURL)
                  .HasMaxLength(400)
                  .IsRequired();


            entity.Property(e => e.ImageOrder)
                  .HasDefaultValue(0)
                  .IsRequired();

            // ==================================================
            // Relationships
            // ==================================================
            entity.HasOne(e => e.ProductCatalog)
                  .WithMany(p => p.ProductImages)
                  .HasForeignKey(e => e.ProductID)
                  .HasConstraintName("FK_ProductImages_ProductCatalog")
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}