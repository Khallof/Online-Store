using Store.Core.Entities;

public partial class ProductImages
{
    public int ImageID { get; set; }
    public string ImageURL { get; set; } = null!;
    public int ProductID { get; set; }
    public short ImageOrder { get; set; }  // ✅ changed from int to short
    public virtual ProductCatalog ProductCatalog { get; set; } = null!;
}