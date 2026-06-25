
using System.ComponentModel.DataAnnotations;


namespace Store.Core.DTOs.Product
{
    public class ImageUploadDto
    {
       
        [Range(0, 100)]
        public int ImageOrder { get; set; } = 0;
    }
}
