using System.ComponentModel.DataAnnotations;

public class EditProductModel
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public decimal? Price { get; set; }
    public int Id { get; set; }
}