using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Repository.Data.Configuration
{
    public class ProductCategoryConfig : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> entity)
        {
            // Unique — no duplicate category names
            entity.HasIndex(e => e.CategoryName, "UQ_ProductCategory_CategoryName")
                  .IsUnique();

            entity.ToTable("ProductCategory");

            entity.HasKey(e => e.CategoryID);


            entity.Property(e => e.CategoryName)
                  .HasMaxLength(100)
                  .IsRequired();
        }
    }
}