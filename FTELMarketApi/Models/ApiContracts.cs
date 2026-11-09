namespace FTELMarketApi.Models;

public record ProductResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int QuantityInStock,
    DateTime CreatedAt,
    int CategoryId,
    string CategoryName);

public record CategoryResponse(int Id, string Name);

public class ProductCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public int CategoryId { get; set; }
}

public class ProductUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public int CategoryId { get; set; }
}

public class CategoryCreateRequest
{
    public string Name { get; set; } = string.Empty;
}
