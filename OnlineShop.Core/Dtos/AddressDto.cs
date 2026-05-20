namespace OnlineShop.Core.Dtos;

public record AddressDto
{
    public string Country { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string? Apartment { get; init; }
}
