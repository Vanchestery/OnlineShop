using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Core.Dtos.Requests;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Core.Services;

public class OrderService : IOrderService
{
    private readonly IOrdersStorage _orders;
    private readonly IShoppingCartStorage _carts;
    private readonly IMapper _mapper;

    public OrderService(
        IOrdersStorage orders,
        IShoppingCartStorage carts,
        IMapper mapper)
    {
        _orders = orders;
        _carts = carts;
        _mapper = mapper;
    }

    public async Task<OrderDto> CreateFromCartAsync(Guid userId, Guid cartId, CreateOrderRequest request, CancellationToken ct = default)
    {
        var cart = await _carts.GetByIdAsync(cartId, ct)
            ?? throw new InvalidOperationException($"Корзина {cartId} не найдена.");

        if (cart.Items.Count == 0)
        {
            throw new InvalidOperationException("Корзина пуста — заказ невозможен.");
        }

        // Snapshot товаров на момент оформления.
        var items = new List<OrderItem>(cart.Items.Count);
        foreach (var ci in cart.Items)
        {
            if (ci.Product is null)
            {
                throw new InvalidOperationException($"Товар {ci.ProductId} не найден.");
            }
            if (!ci.Product.IsAvailable)
            {
                throw new InvalidOperationException($"Товар «{ci.Product.Name}» больше недоступен к покупке.");
            }

            items.Add(new OrderItem
            {
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                Price = ci.Product.Price,
                Quantity = ci.Quantity
            });
        }

        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.Created,
            DeliveryAddress = _mapper.Map<Address>(request.DeliveryAddress),
            Items = items
        };

        await _orders.AddAsync(order, ct);
        await _carts.ClearAsync(cartId, ct);

        // Перечитываем заказ, чтобы навигация на User подтянулась для UserFullName в DTO.
        var saved = await _orders.GetByIdAsync(order.Id, ct);
        return _mapper.Map<OrderDto>(saved ?? order);
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(id, ct);
        return order is null ? null : _mapper.Map<OrderDto>(order);
    }

    public async Task<IReadOnlyList<OrderDto>> GetForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var entities = await _orders.GetByUserAsync(userId, ct);
        return _mapper.Map<List<OrderDto>>(entities);
    }

    public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _orders.GetAllAsync(ct);
        return _mapper.Map<List<OrderDto>>(entities);
    }

    public Task ChangeStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default) =>
        _orders.UpdateStatusAsync(id, status, ct);
}
