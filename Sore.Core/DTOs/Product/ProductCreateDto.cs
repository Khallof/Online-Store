using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Product
{
    public class ProductCreateDto
    {

        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string ProductName { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }

        public List<ProductImageCreateDto> Images { get; set; } = new();
    }
}
