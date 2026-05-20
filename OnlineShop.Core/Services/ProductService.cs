using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Core.Services;

public class ProductService : IProductService
{
    private readonly IProductsStorage _products;
    private readonly IMapper _mapper;

    public ProductService(IProductsStorage products, IMapper mapper)
    {
        _products = products;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(bool includeUnavailable = false, CancellationToken ct = default)
    {
        var entities = await _products.GetAllAsync(includeUnavailable, ct);
        return _mapper.Map<List<ProductDto>>(entities);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _products.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<ProductDto>(entity);
    }

    public async Task<IReadOnlyList<ProductDto>> SearchAsync(string? query, CancellationToken ct = default)
    {
        var entities = await _products.SearchAsync(query, ct);
        return _mapper.Map<List<ProductDto>>(entities);
    }

    public async Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Product>(dto);
        entity.Id = Guid.Empty;  // на всякий случай — пусть БД сгенерирует
        entity.CreatedAt = DateTimeOffset.UtcNow;
        entity.UpdatedAt = entity.CreatedAt;

        await _products.AddAsync(entity, ct);
        return _mapper.Map<ProductDto>(entity);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, ProductDto dto, CancellationToken ct = default)
    {
        var existing = await _products.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Товар {id} не найден.");

        existing.Name = dto.Name;
        existing.Description = dto.Description;
        existing.Price = dto.Price;
        existing.ImagePath = dto.ImagePath;
        existing.IsAvailable = dto.IsAvailable;

        await _products.UpdateAsync(existing, ct);
        return _mapper.Map<ProductDto>(existing);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default) =>
        _products.DeleteAsync(id, ct);
}
