using Microsoft.AspNetCore.Identity;

namespace OnlineShop.Db.Models;

/// <summary>
/// Роль пользователя. Наследуется от IdentityRole&lt;Guid&gt;.
/// </summary>
public class Role : IdentityRole<Guid>
{
    public Role() { }

    public Role(string name) : base(name) { }
}
