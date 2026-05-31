using OnlineShop.Db.Models;

namespace OnlineShop.Core.Dtos;

public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public ProductCategory Category { get; init; } = ProductCategory.Other;
    public string? ImagePath { get; init; }
    public bool IsAvailable { get; init; } = true;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}
