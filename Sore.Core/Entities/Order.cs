

namespace Store.Core.Entities
{
    public partial class Order
    {

        public int OrderID { get; set; }

        public int CustomerID { get; set; }

        public DateTime Orderdate { get; set; }

        public decimal TotalAmount { get; set; } 

        public short Status { get; set; } 


       

        public virtual ICollection<OrderItem> OrderItem { get; set; } =
            new List<OrderItem>();



        public virtual Customer Customer { get; set; } = null!;

        public virtual Payment Payment { get; set; } = null!;

        public virtual Shipping Shipping { get; set; } = null!;


    }
}
