using System.ComponentModel.DataAnnotations;

namespace FTELMarketApi.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
