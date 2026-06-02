using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Shipping
{
    public class ShippingCreateDto
    {
        public int OrderID { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public string? TrackingNumber { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
    }
}
