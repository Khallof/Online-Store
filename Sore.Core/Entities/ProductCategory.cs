

namespace Store.Core.Entities
{
    public partial class ProductCategory
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = null!;


        public virtual ICollection<ProductCatalog> ProductCatalog { get; set; } =
            new List<ProductCatalog>();



    }
}
