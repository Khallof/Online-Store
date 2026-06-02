

namespace Store.Core.Entities
{
    public partial class ProductCatalog
    {

        public int ProductID { get; set; }

        public string ProductName { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int QuantityInStock { get; set; }

      

        public int CategoryID { get; set; }


        public virtual ProductCategory ProductCategory { get; set; } = null!;

        public virtual ICollection<OrderItem> OrderItem { get; set; } =
            new List<OrderItem>();


        public virtual ICollection<Review> Review { get; set; } =
            new List<Review>();

        public virtual ICollection<ProductImages> ProductImages { get; set; } =
            new List<ProductImages>();


    }
}
