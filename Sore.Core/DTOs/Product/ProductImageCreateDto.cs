using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Product
{
    public class ProductImageCreateDto
    {
        [Required(ErrorMessage = "Image URL is required")]
        [MaxLength(400, ErrorMessage = "Image URL cannot exceed 400 characters")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string ImageURL { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Image order must be between 0 and 100")]
        public short ImageOrder { get; set; } = 0;
    }
}
