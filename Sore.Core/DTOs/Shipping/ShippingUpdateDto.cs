using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Shipping
{
    public class ShippingUpdateDto
    {
        public string? TrackingNumber { get; set; }
        // 0=Pending, 1=Shipped, 2=InTransit, 3=Delivered, 4=Failed
        public short ShippingStatus { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
    }
}
