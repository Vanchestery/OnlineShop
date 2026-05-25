using OnlineShop.Core.Dtos;
using OnlineShop.Db.Models;

namespace OnlineShop.Core.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(bool includeUnavailable = false, CancellationToken ct = default);

    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<ProductDto>> SearchAsync(string? query, ProductCategory? category = null, CancellationToken ct = default);

    Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken ct = default);

    Task<ProductDto> UpdateAsync(Guid id, ProductDto dto, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
