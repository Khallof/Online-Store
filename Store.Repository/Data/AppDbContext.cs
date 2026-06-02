using Microsoft.EntityFrameworkCore;

using Store.Core.Entities;
using System.Reflection;


namespace Store.Repository.Data
{
    public partial class AppDbContext : DbContext
    {


        public AppDbContext(DbContextOptions<AppDbContext> options) :
               base(options)
        { }



        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<ProductCatalog> ProductCatalogs { get; set; }

        public virtual DbSet<ProductCategory> ProductCategories { get; set; }


        public virtual DbSet<ProductImages> ProductImages { get; set; }


        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<Shipping> Shippings { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

