using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class UserRepositoryTests
{
    private readonly Mock<ILogger<UserRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _dbOptions;

    public UserRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _dbOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        context.UserInfos.AddRange(
            new UserInfo { Id = 1, Email = "active@test.com", IsActive = true },
            new UserInfo { Id = 2, Email = "inactive@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("active@test.com", result.First().Email);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExistsAndActive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        context.UserInfos.Add(new UserInfo { Id = 1, Email = "test@test.com", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        context.UserInfos.Add(new UserInfo
        {
            Id = 1,
            Email = "find@test.com",
            IsActive = true
        });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("find@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("find@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenInactive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        context.UserInfos.Add(new UserInfo
        {
            Id = 1,
            Email = "inactive@test.com",
            IsActive = false
        });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("inactive@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsAndReturnsUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new UserInfo { Email = "new@test.com", IsActive = true };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("new@test.com", result.Email);
        Assert.Single(context.UserInfos);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new UserInfo { Email = "original@test.com", IsActive = true };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Act
        user.Email = "updated@test.com";
        await repository.UpdateAsync(user);

        // Assert
        var updated = await context.UserInfos.FindAsync(user.Id);
        Assert.Equal("updated@test.com", updated?.Email);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new UserInfo { Id = 1, Email = "delete@test.com", IsActive = true };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(1);

        // Assert
        var deleted = await context.UserInfos.FindAsync(1);
        Assert.NotNull(deleted);
        Assert.False(deleted.IsActive);
        Assert.NotNull(deleted.ModifiedDate);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenActiveExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        context.UserInfos.Add(new UserInfo { Id = 1, Email = "exists@test.com", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ReturnsTrue_WhenEmailExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        context.UserInfos.Add(new UserInfo
        {
            Email = "exists@test.com",
            IsActive = true
        });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.EmailExistsAsync("exists@test.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ReturnsFalse_WhenEmailNotExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.EmailExistsAsync("notfound@test.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ReturnsFalse_WhenEmailExistsButInactive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        context.UserInfos.Add(new UserInfo
        {
            Email = "inactive@test.com",
            IsActive = false
        });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.EmailExistsAsync("inactive@test.com");

        // Assert
        Assert.False(result);
    }
}
