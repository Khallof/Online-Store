using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Shipping
{
    public class ShippingUpdateDto
    {
        [MaxLength(50, ErrorMessage = "Tracking number cannot exceed 50 characters")]
        public string? TrackingNumber { get; set; }

        [Range(0, 4, ErrorMessage = "Status must be between 0 and 4")]
        public short ShippingStatus { get; set; }

        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
    }
}
