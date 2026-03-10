using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for user operations
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserInfo>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<UserInfo?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<string> RegisterUserAsync(UserInfo user, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(string email, UserInfo user, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserInfo>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default);
}
