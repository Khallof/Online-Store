using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Repository.Data.Configuration
{
    public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> entity)
        {
            // ==================================================
            // Primary Key
            // ==================================================
            entity.HasKey(e => e.Id);

            // ==================================================
            // Properties
            // ==================================================
            entity.Property(e => e.Token)
                  .HasMaxLength(500)
                  .IsRequired();

            entity.Property(e => e.ExpiresAt)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .IsRequired()
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.RevokedAt);  // nullable

            // ==================================================
            // Indexes
            // ==================================================
            entity.HasIndex(e => e.Token)
                  .IsUnique();

            entity.HasIndex(e => e.CustomerID);

            // ==================================================
            // Relationships
            // ==================================================
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerID)
                  .HasConstraintName("FK_RefreshTokens_Customers")
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
