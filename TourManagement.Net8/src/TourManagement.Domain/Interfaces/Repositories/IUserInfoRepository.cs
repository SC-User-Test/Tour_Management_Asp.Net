using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserInfo entity operations
/// </summary>
public interface IUserInfoRepository
{
    Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserInfo?> GetByIdAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo> AddAsync(UserInfo user, CancellationToken cancellationToken = default);
    Task<UserInfo> UpdateAsync(UserInfo user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
}
