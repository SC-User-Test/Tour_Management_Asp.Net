using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.UnitTests.TourManagement.Application.Services;

public class UserInfoServiceTests
{
    private readonly Mock<IUserInfoRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UserInfoService>> _mockLogger;
    private readonly UserInfoService _service;

    public UserInfoServiceTests()
    {
        _mockRepository = new Mock<IUserInfoRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UserInfoService>>();
        _service = new UserInfoService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new UserInfoService(null!, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullMapper_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new UserInfoService(_mockRepository.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new UserInfoService(_mockRepository.Object, _mockMapper.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsUserInfoDtos()
    {
        var users = new List<UserInfo> { new UserInfo { Email = "test@test.com" } };
        var userDtos = new List<UserInfoDto> { new UserInfoDto { Email = "test@test.com" } };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);
        _mockMapper.Setup(m => m.Map<IEnumerable<UserInfoDto>>(users)).Returns(userDtos);

        var result = await _service.GetAllAsync();

        Assert.Equal(userDtos.Count, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidEmail_ReturnsUserInfoDto()
    {
        var user = new UserInfo { Email = "test@test.com", FirstName = "John" };
        var userDto = new UserInfoDto { Email = "test@test.com", FirstName = "John" };
        _mockRepository.Setup(r => r.GetByIdAsync("test@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserInfoDto>(user)).Returns(userDto);

        var result = await _service.GetByIdAsync("test@test.com");

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidEmail_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByIdAsync("invalid@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        var result = await _service.GetByIdAsync("invalid@test.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesUser()
    {
        var createDto = new UserInfoCreateDto { Email = "new@test.com", FirstName = "John", LastName = "Doe" };
        var user = new UserInfo { Email = "new@test.com", FirstName = "John" };
        var userDto = new UserInfoDto { Email = "new@test.com", FirstName = "John" };
        _mockRepository.Setup(r => r.ExistsAsync("new@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockMapper.Setup(m => m.Map<UserInfo>(createDto)).Returns(user);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserInfoDto>(user)).Returns(userDto);

        var result = await _service.CreateAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("new@test.com", result.Email);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyEmail_ThrowsArgumentException()
    {
        var createDto = new UserInfoCreateDto { Email = "", FirstName = "John" };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        var createDto = new UserInfoCreateDto { Email = "existing@test.com", FirstName = "John" };
        _mockRepository.Setup(r => r.ExistsAsync("existing@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesUser()
    {
        var updateDto = new UserInfoUpdateDto { FirstName = "Updated", LastName = "Name" };
        var existingUser = new UserInfo { Email = "test@test.com", FirstName = "Old" };
        var updatedUser = new UserInfo { Email = "test@test.com", FirstName = "Updated" };
        var userDto = new UserInfoDto { Email = "test@test.com", FirstName = "Updated" };
        _mockRepository.Setup(r => r.GetByIdAsync("test@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);
        _mockMapper.Setup(m => m.Map(updateDto, existingUser)).Returns(existingUser);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedUser);
        _mockMapper.Setup(m => m.Map<UserInfoDto>(updatedUser)).Returns(userDto);

        var result = await _service.UpdateAsync("test@test.com", updateDto);

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidEmail_ThrowsInvalidOperationException()
    {
        var updateDto = new UserInfoUpdateDto { FirstName = "Updated" };
        _mockRepository.Setup(r => r.GetByIdAsync("invalid@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync("invalid@test.com", updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WithValidEmail_ReturnsTrue()
    {
        _mockRepository.Setup(r => r.ExistsAsync("test@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync("test@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.DeleteAsync("test@test.com");

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidEmail_ReturnsFalse()
    {
        _mockRepository.Setup(r => r.ExistsAsync("invalid@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _service.DeleteAsync("invalid@test.com");

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingUsers()
    {
        var users = new List<UserInfo> { new UserInfo { Email = "john@test.com", FirstName = "John" } };
        var userDtos = new List<UserInfoDto> { new UserInfoDto { Email = "john@test.com", FirstName = "John" } };
        _mockRepository.Setup(r => r.SearchAsync("John", It.IsAny<CancellationToken>())).ReturnsAsync(users);
        _mockMapper.Setup(m => m.Map<IEnumerable<UserInfoDto>>(users)).Returns(userDtos);

        var result = await _service.SearchAsync("John");

        Assert.Single(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ReturnsTrue()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new UserInfo { Email = "test@test.com", PasswordHash = passwordHash };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.ValidateLoginAsync("test@test.com", "password123");

        Assert.True(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInvalidPassword_ReturnsFalse()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new UserInfo { Email = "test@test.com", PasswordHash = passwordHash };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.ValidateLoginAsync("test@test.com", "wrongpassword");

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithNonExistentUser_ReturnsFalse()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync("nonexistent@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        var result = await _service.ValidateLoginAsync("nonexistent@test.com", "password123");

        Assert.False(result);
    }
}
