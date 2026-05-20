using OnlineShop.Core.Dtos;

namespace OnlineShop.Core.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(bool includeUnavailable = false, CancellationToken ct = default);

    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<ProductDto>> SearchAsync(string? query, CancellationToken ct = default);

    Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken ct = default);

    Task<ProductDto> UpdateAsync(Guid id, ProductDto dto, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
