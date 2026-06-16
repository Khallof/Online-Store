using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Shipping
{
    public class ShippingCreateDto
    {
        [Required(ErrorMessage = "Order is required")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Carrier name is required")]
        [MaxLength(100, ErrorMessage = "Carrier name cannot exceed 100 characters")]
        public string CarrierName { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Tracking number cannot exceed 50 characters")]
        public string? TrackingNumber { get; set; }

        public DateTime? EstimatedDeliveryDate { get; set; }
    }
}
