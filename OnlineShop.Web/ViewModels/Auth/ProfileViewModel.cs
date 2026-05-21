namespace OnlineShop.Web.ViewModels.Auth;

public class ProfileViewModel
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}
