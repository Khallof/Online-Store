using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Order
{
    public class OrderItemCreateDto
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }
}
