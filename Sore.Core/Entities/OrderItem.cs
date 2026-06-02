

namespace Store.Core.Entities
{
    public partial class OrderItem
    {

        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public  decimal TotalItemsPrice {  get; set; }


        public virtual Order Order { get; set; } = null!;

        public virtual ProductCatalog ProductCatalog { get; set; } = null!;



    }
}
