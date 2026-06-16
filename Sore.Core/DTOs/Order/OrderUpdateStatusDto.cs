using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Order
{
    public class OrderUpdateStatusDto
    {
        [Required(ErrorMessage = "Status is required")]
        [Range(0, 4, ErrorMessage = "Status must be between 0 and 4")]
        public short Status { get; set; }
    }
}
