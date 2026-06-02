

namespace Store.Core.Entities
{
    public partial class Review
    {

        public int ReviewID { get; set; }

        public int ProductID { get; set; } 

        public int CustomerID { get; set; }

        public string? ReviewText { get; set; }

        public decimal Rating { get; set; } 

        public DateTime ReviewDate { get; set; } 


        public virtual Customer Customer { get; set; } = null!;


        public virtual ProductCatalog ProductCatalog { get; set; } = null!;


    }
}
