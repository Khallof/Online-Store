using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Store.Repository.Data.Configuration
{
    public  class CustomerConfig : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> entity)
        {
            entity.HasIndex(e => e.Email, "IX_Customers_Email")
                       .IsUnique();

            entity.HasIndex(e => e.Username, "UQ_Customers_Username")
                  .IsUnique();

            entity.HasKey(e => e.CustomerID);

            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Password).HasMaxLength(100).IsRequired();

            entity.Property(e => e.Phone)
           .HasMaxLength(20);

            entity.Property(e => e.Address)
           .HasMaxLength(200);

            entity.Property(e => e.Role)
                 .HasMaxLength(20)
                 .IsRequired()
                 .HasDefaultValue("Customer");

        }

    }
}
