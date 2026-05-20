namespace OnlineShop.Db.Models;

/// <summary>
/// Адрес доставки. Owned-type — встраивается в таблицу-владельца (Orders) как набор
/// колонок DeliveryAddress_Country, DeliveryAddress_City, ... без отдельной таблицы.
/// </summary>
public class Address
{
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string? Apartment { get; set; }
}
