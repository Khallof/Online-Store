using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Order
{
    public class OrderCreateDto
    {
        public int CustomerID { get; set; }
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }
}
