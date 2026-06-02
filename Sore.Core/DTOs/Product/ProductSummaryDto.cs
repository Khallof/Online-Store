using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Product
{
    public class ProductSummaryDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public string CategoryName { get; set; } = string.Empty;  // flattened from category
        public string? ThumbnailUrl { get; set; }                    // first image (ImageOrder = 0)
    }
}
