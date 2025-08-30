using System.ComponentModel.DataAnnotations;

namespace Role_Base_Product_Management_System.Models
{
    public class CreateProductModel
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public decimal? Price { get; set; }
    }
}