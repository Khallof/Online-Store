

namespace Store.Core.Entities
{
    public partial class Shipping
    {
        public int ShippingID { get; set; }

        public int OrderID { get; set; }

        public string CarrierName { get; set; } = null!;

        public string? TrackingNumber { get; set; }

        public short ShippingStatus { get; set; } 

        public DateTime? EstimatedDeliveryDate { get; set; } 
          

        public DateTime? ActualDeliveryDate { get; set; }
           



        public virtual Order Order { get; set; } = null!;


    }



    
}
