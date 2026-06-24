using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Infrastructure.Data;

namespace TownHubPeople.Infrastructure.Services;

public class UserLookupService : IUserLookupService
{
    private readonly ApplicationDbContext _context;

    public UserLookupService(ApplicationDbContext context) => _context = context;

    public async Task<UserContact?> FindAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserContact(u.Email, u.FirstName + " " + u.LastName))
            .FirstOrDefaultAsync(ct);
    }
}
