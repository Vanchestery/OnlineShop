using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Db.Storage;

public class UsersStorage : IUsersStorage
{
    private readonly ApplicationDbContext _db;

    public UsersStorage(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Users.AsNoTracking().OrderBy(u => u.Email).ToListAsync(ct);
}
