using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Order
{
    public class OrderCreateDto
    {
        [Required(ErrorMessage = "Customer is required")]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Order must have at least one item")]
        [MinLength(1, ErrorMessage = "Order must have at least one item")]
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }
}
