namespace OnlineShop.Core.Dtos.Requests;

public record CreateOrderRequest
{
    public AddressDto DeliveryAddress { get; init; } = new();
}
