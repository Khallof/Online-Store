using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Core.DTOs.Payment;
using Store.Core.DTOs.Shipping;

namespace Store.Core.DTOs.Order
{
    public class OrderDetailDto
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;  // flattened from customer
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
        public PaymentDto? Payment { get; set; }
        public ShippingDto? Shipping { get; set; }
    }
}
