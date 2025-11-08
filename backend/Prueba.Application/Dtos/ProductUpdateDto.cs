using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace Prueba.Application.Dtos
{
    public class ProductUpdateDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public IFormFile Image { get; set; }
    }
}