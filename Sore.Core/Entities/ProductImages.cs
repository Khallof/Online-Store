

namespace Store.Core.Entities
{
    public partial class ProductImages
    {
        public int ImageID { get; set; }

        public string ImageURL { get; set; } = null!;

        public int ProductID { get; set; }

        public int ImageOrder { get; set; }


        public virtual ProductCatalog ProductCatalog { get; set; } 
            = null!;
    }
}
