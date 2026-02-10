using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for UserInfo operations
/// </summary>
public class UserInfoService : IUserInfoService
{
    private readonly IUserInfoRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserInfoService> _logger;

    public UserInfoService(IUserInfoRepository repository, IMapper mapper, ILogger<UserInfoService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserInfoDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            var users = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<UserInfoDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    public async Task<UserInfoDto?> GetByIdAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email {Email}", email);
            var user = await _repository.GetByIdAsync(email, cancellationToken);
            return user == null ? null : _mapper.Map<UserInfoDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            throw;
        }
    }

    public async Task<UserInfoDto> CreateAsync(UserInfoCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user: {Email}", dto.Email);

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required", nameof(dto.Email));

            if (await _repository.ExistsAsync(dto.Email, cancellationToken))
                throw new InvalidOperationException($"User with email {dto.Email} already exists");

            var user = _mapper.Map<UserInfo>(dto);
            var createdUser = await _repository.AddAsync(user, cancellationToken);

            _logger.LogInformation("User created successfully with email {Email}", createdUser.Email);
            return _mapper.Map<UserInfoDto>(createdUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", dto.Email);
            throw;
        }
    }

    public async Task<UserInfoDto> UpdateAsync(string email, UserInfoUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with email {Email}", email);

            var existingUser = await _repository.GetByIdAsync(email, cancellationToken);
            if (existingUser == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                throw new InvalidOperationException($"User with email {email} not found");
            }

            _mapper.Map(dto, existingUser);
            existingUser.Email = email;

            var updatedUser = await _repository.UpdateAsync(existingUser, cancellationToken);

            _logger.LogInformation("User with email {Email} updated successfully", email);
            return _mapper.Map<UserInfoDto>(updatedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with email {Email}", email);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with email {Email}", email);

            var exists = await _repository.ExistsAsync(email, cancellationToken);
            if (!exists)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return false;
            }

            var result = await _repository.DeleteAsync(email, cancellationToken);

            if (result)
                _logger.LogInformation("User with email {Email} deleted successfully", email);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with email {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<UserInfoDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            var users = await _repository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<UserInfoDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<bool> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for user {Email}", email);

            var user = await _repository.GetByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return false;
            }

            var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (isValid)
                _logger.LogInformation("Login validation successful for user {Email}", email);
            else
                _logger.LogWarning("Login validation failed for user {Email}", email);

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for user {Email}", email);
            throw;
        }
    }
}
