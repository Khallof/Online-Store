using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Order
{
    public class OrderItemDto
    {
        public int OrderItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;   // flattened from product
        public string? ThumbnailUrl { get; set; }                 // first image of product
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalItemsPrice { get; set; }
    }
}
