namespace OnlineShop.Db;

/// <summary>
/// Имена ролей. Используются в [Authorize(Roles = ...)] и при сидинге.
/// </summary>
public static class RoleNames
{
    public const string Admin = "Admin";
    public const string User = "User";
}
