using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for UserInfo entity
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly TourManagementDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(TourManagementDbContext context, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<UserInfo?> GetByIdAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<UserInfo?> GetByEmailAndPasswordAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.IsActive, cancellationToken);
    }

    public async Task AddAsync(UserInfo user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserInfo user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { email }, cancellationToken);
        if (user != null)
        {
            user.IsActive = false;
            user.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.IsActive &&
                (u.Email.Contains(searchTerm) ||
                 u.FirstName.Contains(searchTerm) ||
                 u.LastName.Contains(searchTerm)))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
