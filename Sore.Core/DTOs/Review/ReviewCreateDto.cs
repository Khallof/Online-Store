using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Review
{
    public class ReviewCreateDto
    {
        public int ProductID { get; set; }
        public int CustomerID { get; set; }
        public string? ReviewText { get; set; }
        public decimal Rating { get; set; }  // must be between 1.0 and 5.0
    }
}
