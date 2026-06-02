using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Product
{
    public class ProductImageDto
    {
        public int ImageID { get; set; }
        public string ImageURL { get; set; } = string.Empty;
        public int ImageOrder { get; set; }
    }
}
