using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.UnitTests.TourManagement.Infrastructure.Repositories;

public class UserInfoRepositoryTests
{
    private readonly Mock<ILogger<UserInfoRepository>> _mockLogger;
    private readonly TourManagementDbContext _context;
    private readonly UserInfoRepository _repository;

    public UserInfoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new TourManagementDbContext(options);
        _mockLogger = new Mock<ILogger<UserInfoRepository>>();
        _repository = new UserInfoRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new UserInfoRepository(null!, _mockLogger.Object));
        Assert.Equal("context", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new UserInfoRepository(_context, null!));
        Assert.Equal("logger", exception.ParamName);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveUsers()
    {
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "user1@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash1", DateOfBirth = DateTime.Now, Street = "St1", City = "City1", State = "State1" },
            new UserInfo { Email = "user2@test.com", FirstName = "Jane", LastName = "Smith", IsActive = true, Gender = "Female", PasswordHash = "hash2", DateOfBirth = DateTime.Now, Street = "St2", City = "City2", State = "State2" },
            new UserInfo { Email = "user3@test.com", FirstName = "Bob", LastName = "Jones", IsActive = false, Gender = "Male", PasswordHash = "hash3", DateOfBirth = DateTime.Now, Street = "St3", City = "City3", State = "State3" }
        };
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.All(result, user => Assert.True(user.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_ReturnsList()
    {
        var user = new UserInfo { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidEmail_ReturnsUser()
    {
        var user = new UserInfo { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync("test@test.com");

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidEmail_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync("nonexistent@test.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveUser_ReturnsNull()
    {
        var user = new UserInfo { Email = "inactive@test.com", FirstName = "Test", LastName = "User", IsActive = false, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync("inactive@test.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
    {
        var user = new UserInfo { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByEmailAsync("test@test.com");

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
    {
        var result = await _repository.GetByEmailAsync("nonexistent@test.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WithValidUser_AddsUser()
    {
        var user = new UserInfo { Email = "new@test.com", FirstName = "New", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };

        var result = await _repository.AddAsync(user);

        Assert.NotNull(result);
        Assert.Equal("new@test.com", result.Email);
        Assert.Single(_context.Users);
    }

    [Fact]
    public async Task UpdateAsync_WithValidUser_UpdatesUser()
    {
        var user = new UserInfo { Email = "update@test.com", FirstName = "Old", LastName = "Name", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        _context.Entry(user).State = EntityState.Detached;

        user.FirstName = "New";
        var result = await _repository.UpdateAsync(user);

        Assert.Equal("New", result.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_WithValidEmail_SoftDeletesUser()
    {
        var user = new UserInfo { Email = "delete@test.com", FirstName = "Delete", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.DeleteAsync("delete@test.com");

        Assert.True(result);
        var deletedUser = await _context.Users.FindAsync("delete@test.com");
        Assert.False(deletedUser!.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidEmail_ReturnsFalse()
    {
        var result = await _repository.DeleteAsync("nonexistent@test.com");

        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingUser_ReturnsTrue()
    {
        var user = new UserInfo { Email = "exists@test.com", FirstName = "Exists", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync("exists@test.com");

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingUser_ReturnsFalse()
    {
        var result = await _repository.ExistsAsync("nonexistent@test.com");

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithEmptyTerm_ReturnsAllUsers()
    {
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "user1@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash1", DateOfBirth = DateTime.Now, Street = "St1", City = "City1", State = "State1" },
            new UserInfo { Email = "user2@test.com", FirstName = "Jane", LastName = "Smith", IsActive = true, Gender = "Female", PasswordHash = "hash2", DateOfBirth = DateTime.Now, Street = "St2", City = "City2", State = "State2" }
        };
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ReturnsMatchingUsers()
    {
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "john@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash1", DateOfBirth = DateTime.Now, Street = "St1", City = "City1", State = "State1" },
            new UserInfo { Email = "jane@test.com", FirstName = "Jane", LastName = "Smith", IsActive = true, Gender = "Female", PasswordHash = "hash2", DateOfBirth = DateTime.Now, Street = "St2", City = "City2", State = "State2" }
        };
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("John");

        Assert.Single(result);
        Assert.Equal("john@test.com", result.First().Email);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithValidCredentials_ReturnsTrue()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new UserInfo { Email = "validate@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = passwordHash, DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.ValidateCredentialsAsync("validate@test.com", "password123");

        Assert.True(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithInvalidPassword_ReturnsFalse()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new UserInfo { Email = "validate@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = passwordHash, DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.ValidateCredentialsAsync("validate@test.com", "wrongpassword");

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithNonExistentUser_ReturnsFalse()
    {
        var result = await _repository.ValidateCredentialsAsync("nonexistent@test.com", "password123");

        Assert.False(result);
    }
}
