using System;
using System.ComponentModel.DataAnnotations;

namespace Role_Base_Product_Management_System.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        // store protected/encrypted price string
        public string? EncryptedPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
