using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Review
{
    public class ReviewCreateDto
    {
        [Required(ErrorMessage = "Product is required")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        public int CustomerID { get; set; }

        [MaxLength(500, ErrorMessage = "Review text cannot exceed 500 characters")]
        public string? ReviewText { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1.0, 5.0, ErrorMessage = "Rating must be between 1.0 and 5.0")]
        public decimal Rating { get; set; }
    }
}
