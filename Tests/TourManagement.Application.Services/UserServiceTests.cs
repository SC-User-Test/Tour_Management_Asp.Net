using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _service = new UserService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<UserInfo>
        {
            new UserInfo { Id = 1, Email = "user1@test.com" },
            new UserInfo { Id = 2, Email = "user2@test.com" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(users);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExists()
    {
        // Arrange
        var user = new UserInfo { Id = 1, Email = "test@test.com" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(user);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@test.com", result.Email);
        _mockRepository.Verify(r => r.GetByIdAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenExists()
    {
        // Arrange
        var user = new UserInfo { Id = 1, Email = "test@test.com" };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com", default)).ReturnsAsync(user);

        // Act
        var result = await _service.GetByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
        _mockRepository.Verify(r => r.GetByEmailAsync("test@test.com", default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_HashesPasswordAndSetsDefaults()
    {
        // Arrange
        var user = new UserInfo { Email = "new@test.com", FirstName = "John" };
        _mockRepository.Setup(r => r.EmailExistsAsync("new@test.com", default)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), default))
            .ReturnsAsync((UserInfo u, CancellationToken ct) => u);

        // Act
        var result = await _service.CreateAsync(user, "password123");

        // Assert
        Assert.NotNull(result.PasswordHash);
        Assert.NotEqual("password123", result.PasswordHash);
        Assert.True(result.IsActive);
        Assert.NotEqual(default(DateTime), result.CreatedDate);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<UserInfo>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenEmailExists()
    {
        // Arrange
        var user = new UserInfo { Email = "existing@test.com" };
        _mockRepository.Setup(r => r.EmailExistsAsync("existing@test.com", default)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.CreateAsync(user, "password123"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<UserInfo>(), default), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedDate()
    {
        // Arrange
        var user = new UserInfo { Id = 1, Email = "updated@test.com" };
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(user);

        // Assert
        Assert.NotEqual(default(DateTime), user.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<UserInfo>(u =>
            u.ModifiedDate != null), default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepository()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsUser_WhenCredentialsValid()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new UserInfo
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = passwordHash,
            IsActive = true
        };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com", default)).ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync("test@test.com", "password123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsNull_WhenPasswordInvalid()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new UserInfo
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = passwordHash,
            IsActive = true
        };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com", default)).ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync("test@test.com", "wrongpassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsNull_WhenUserNotActive()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new UserInfo
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = passwordHash,
            IsActive = false
        };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com", default)).ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync("test@test.com", "password123");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("notfound@test.com", default))
            .ReturnsAsync((UserInfo?)null);

        // Act
        var result = await _service.AuthenticateAsync("notfound@test.com", "password123");

        // Assert
        Assert.Null(result);
    }
}
