using System.ComponentModel.DataAnnotations;
namespace Role_Base_Product_Management_System.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+",
            ErrorMessage = "Password must have uppercase, digit, and special character.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; }
    }
}