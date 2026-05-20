using OnlineShop.Core.Dtos;
using OnlineShop.Core.Dtos.Requests;
using OnlineShop.Db.Models;

namespace OnlineShop.Core.Interfaces;

public interface IOrderService
{
    /// <summary>
    /// Оформить заказ из корзины: снимок товаров (Name + Price), очистка корзины
    /// после успешного оформления. Проверки: корзина не пуста, все товары IsAvailable.
    /// </summary>
    Task<OrderDto> CreateFromCartAsync(Guid userId, Guid cartId, CreateOrderRequest request, CancellationToken ct = default);

    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<OrderDto>> GetForUserAsync(Guid userId, CancellationToken ct = default);

    Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken ct = default);

    Task ChangeStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default);
}
