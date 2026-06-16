using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Order
{
    public class OrderSummaryDto
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public short Status { get; set; } = 0;        // readable label e.g. "Delivered"
        public int ItemCount { get; set; }
    }
}
