using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for user operations
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserInfo>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            return await _userRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw new TourManagementException("Error retrieving users", ex);
        }
    }

    public async Task<UserInfo?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email: {Email}", email);
            return await _userRepository.GetByIdAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email: {Email}", email);
            throw new TourManagementException($"Error retrieving user with email: {email}", ex);
        }
    }

    public async Task<UserInfo?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Authenticating user: {Email}", email);

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Authentication failed: empty email or password");
                return null;
            }

            var user = await _userRepository.GetByEmailAndPasswordAsync(email, password, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Authentication failed for user: {Email}", email);
                return null;
            }

            _logger.LogInformation("User authenticated successfully: {Email}", email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user: {Email}", email);
            throw new TourManagementException($"Error authenticating user: {email}", ex);
        }
    }

    public async Task<string> RegisterUserAsync(UserInfo user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Registering new user: {Email}", user.Email);

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ValidationException("Email is required");
            }

            var existingUser = await _userRepository.ExistsAsync(user.Email, cancellationToken);
            if (existingUser)
            {
                throw new ValidationException($"User with email {user.Email} already exists");
            }

            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;

            await _userRepository.AddAsync(user, cancellationToken);

            _logger.LogInformation("User registered successfully: {Email}", user.Email);
            return user.Email;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Email}", user.Email);
            throw new TourManagementException($"Error registering user: {user.Email}", ex);
        }
    }

    public async Task UpdateUserAsync(string email, UserInfo user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user: {Email}", email);

            var existingUser = await _userRepository.GetByIdAsync(email, cancellationToken);
            if (existingUser == null)
            {
                throw new EntityNotFoundException(nameof(UserInfo), email);
            }

            user.Email = email;
            user.ModifiedDate = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("User updated successfully: {Email}", email);
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {Email}", email);
            throw new TourManagementException($"Error updating user: {email}", ex);
        }
    }

    public async Task DeleteUserAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user: {Email}", email);

            var existingUser = await _userRepository.GetByIdAsync(email, cancellationToken);
            if (existingUser == null)
            {
                throw new EntityNotFoundException(nameof(UserInfo), email);
            }

            await _userRepository.DeleteAsync(email, cancellationToken);

            _logger.LogInformation("User deleted successfully: {Email}", email);
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {Email}", email);
            throw new TourManagementException($"Error deleting user: {email}", ex);
        }
    }

    public async Task<IEnumerable<UserInfo>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            return await _userRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw new TourManagementException($"Error searching users with term: {searchTerm}", ex);
        }
    }
}
