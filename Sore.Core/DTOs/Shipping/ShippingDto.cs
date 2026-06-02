using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Shipping
{
    public class ShippingDto
    {
        public int ShippingID { get; set; }
        public int OrderID { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public string? TrackingNumber { get; set; }
        public string Status { get; set; } = string.Empty;        // readable label e.g. "InTransit"
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
    }
}
