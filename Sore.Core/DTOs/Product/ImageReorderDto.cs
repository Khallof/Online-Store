
using System.ComponentModel.DataAnnotations;

namespace Store.Core.DTOs.Product
{
    public class ImageReorderDto
    {
        [Required]
        public int ImageID { get; set; }

        [Required]
        [Range(0, 100)]
        public short ImageOrder { get; set; }
    }
}
