using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Core.Dtos.Requests;
using OnlineShop.Core.Services;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrdersStorage> _orders = new();
    private readonly Mock<IShoppingCartStorage> _carts = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(_orders.Object, _carts.Object, _mapper.Object);

        // Базовая настройка mapper'а — возвращает простые объекты, чтобы
        // не падать на null. Для специфичных тестов перенастраиваем.
        _mapper.Setup(m => m.Map<Address>(It.IsAny<AddressDto>()))
               .Returns(new Address());
        _mapper.Setup(m => m.Map<OrderDto>(It.IsAny<Order>()))
               .Returns(new OrderDto());
    }

    [Fact]
    public async Task CreateFromCart_CartNotFound_ThrowsInvalidOperation()
    {
        var cartId = Guid.NewGuid();
        _carts.Setup(c => c.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
              .ReturnsAsync((Cart?)null);

        var act = () => _sut.CreateFromCartAsync(
            Guid.NewGuid(), cartId,
            new CreateOrderRequest { DeliveryAddress = new AddressDto() });

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*не найдена*");
    }

    [Fact]
    public async Task CreateFromCart_EmptyCart_ThrowsInvalidOperation()
    {
        var userId = Guid.NewGuid();
        var cartId = Guid.NewGuid();
        var emptyCart = new Cart { Id = cartId, UserId = userId, Items = [] };

        _carts.Setup(c => c.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
              .ReturnsAsync(emptyCart);

        var act = () => _sut.CreateFromCartAsync(
            userId, cartId,
            new CreateOrderRequest { DeliveryAddress = new AddressDto() });

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*пуст*");
    }

    [Fact]
    public async Task CreateFromCart_ProductBecameUnavailable_ThrowsInvalidOperation()
    {
        var userId = Guid.NewGuid();
        var cartId = Guid.NewGuid();
        var unavailable = new Product
        {
            Id = Guid.NewGuid(),
            Name = "DiscontinuedItem",
            Price = 100m,
            IsAvailable = false
        };
        var cart = new Cart
        {
            Id = cartId,
            UserId = userId,
            Items = new List<CartItem>
            {
                new() { ProductId = unavailable.Id, Product = unavailable, Quantity = 1 }
            }
        };

        _carts.Setup(c => c.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
              .ReturnsAsync(cart);

        var act = () => _sut.CreateFromCartAsync(
            userId, cartId,
            new CreateOrderRequest { DeliveryAddress = new AddressDto() });

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*DiscontinuedItem*");
    }

    [Fact]
    public async Task CreateFromCart_HappyPath_FreezesProductSnapshot()
    {
        // КЛЮЧЕВОЙ ТЕСТ: при оформлении ProductName и Price копируются
        // в OrderItem (frozen snapshot). Это защищает старые заказы от
        // изменений цены/имени товара в будущем.
        var userId = Guid.NewGuid();
        var cartId = Guid.NewGuid();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Premium Coffee",
            Price = 1290m,
            IsAvailable = true
        };
        var cart = new Cart
        {
            Id = cartId,
            UserId = userId,
            Items = new List<CartItem>
            {
                new() { ProductId = product.Id, Product = product, Quantity = 2 }
            }
        };

        _carts.Setup(c => c.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
              .ReturnsAsync(cart);

        await _sut.CreateFromCartAsync(
            userId, cartId,
            new CreateOrderRequest { DeliveryAddress = new AddressDto() });

        _orders.Verify(o => o.AddAsync(It.Is<Order>(ord =>
            ord.UserId == userId
            && ord.Status == OrderStatus.Created
            && ord.Items.Count == 1
            && ord.Items.First().ProductId == product.Id
            && ord.Items.First().ProductName == "Premium Coffee"
            && ord.Items.First().Price == 1290m
            && ord.Items.First().Quantity == 2
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateFromCart_HappyPath_ClearsCartAfterAdd()
    {
        var userId = Guid.NewGuid();
        var cartId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Name = "X", Price = 10m, IsAvailable = true };
        var cart = new Cart
        {
            Id = cartId,
            UserId = userId,
            Items = new List<CartItem>
            {
                new() { ProductId = product.Id, Product = product, Quantity = 1 }
            }
        };

        _carts.Setup(c => c.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
              .ReturnsAsync(cart);

        await _sut.CreateFromCartAsync(
            userId, cartId,
            new CreateOrderRequest { DeliveryAddress = new AddressDto() });

        _carts.Verify(c => c.ClearAsync(cartId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateFromCart_MultipleItems_SnapshotsAll()
    {
        var userId = Guid.NewGuid();
        var cartId = Guid.NewGuid();
        var p1 = new Product { Id = Guid.NewGuid(), Name = "Coffee", Price = 1000m, IsAvailable = true };
        var p2 = new Product { Id = Guid.NewGuid(), Name = "Tea", Price = 500m, IsAvailable = true };
        var cart = new Cart
        {
            Id = cartId,
            UserId = userId,
            Items = new List<CartItem>
            {
                new() { ProductId = p1.Id, Product = p1, Quantity = 1 },
                new() { ProductId = p2.Id, Product = p2, Quantity = 3 }
            }
        };

        _carts.Setup(c => c.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
              .ReturnsAsync(cart);

        await _sut.CreateFromCartAsync(
            userId, cartId,
            new CreateOrderRequest { DeliveryAddress = new AddressDto() });

        _orders.Verify(o => o.AddAsync(It.Is<Order>(ord =>
            ord.Items.Count == 2
            && ord.Items.Any(i => i.ProductName == "Coffee" && i.Price == 1000m && i.Quantity == 1)
            && ord.Items.Any(i => i.ProductName == "Tea" && i.Price == 500m && i.Quantity == 3)
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}
