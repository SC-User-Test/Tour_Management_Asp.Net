using TourManagement.Domain.DTOs;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for UserInfo operations
/// </summary>
public interface IUserInfoService
{
    Task<IEnumerable<UserInfoDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserInfoDto?> GetByIdAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfoDto> CreateAsync(UserInfoCreateDto dto, CancellationToken cancellationToken = default);
    Task<UserInfoDto> UpdateAsync(string email, UserInfoUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserInfoDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
