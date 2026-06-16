using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.ProductCategory
{
    public class ProductCategoryCreateDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        [MinLength(2, ErrorMessage = "Category name must be at least 2 characters")]
        public string CategoryName { get; set; } = string.Empty;
    }
}
