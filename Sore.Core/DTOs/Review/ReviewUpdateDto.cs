using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Review
{
    public class ReviewUpdateDto
    {
        public string? ReviewText { get; set; }
        public decimal Rating { get; set; }
    }
}
