using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class TourRepositoryTests
{
    private readonly Mock<ILogger<TourRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _dbOptions;

    public TourRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<TourRepository>>();
        _dbOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.AddRange(
            new Tour { Id = 1, TourName = "Active Tour", IsActive = true },
            new Tour { Id = 2, TourName = "Inactive Tour", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Active Tour", result.First().TourName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTour_WhenExistsAndActive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.Add(new Tour { Id = 1, TourName = "Test Tour", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenInactive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.Add(new Tour { Id = 1, TourName = "Inactive", IsActive = false });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsAndReturnsTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "New Tour", IsActive = true };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("New Tour", result.TourName);
        Assert.Single(context.Tours);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "Original", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        tour.TourName = "Updated";
        await repository.UpdateAsync(tour);

        // Assert
        var updated = await context.Tours.FindAsync(tour.Id);
        Assert.Equal("Updated", updated?.TourName);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { Id = 1, TourName = "To Delete", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(1);

        // Assert
        var deleted = await context.Tours.FindAsync(1);
        Assert.NotNull(deleted);
        Assert.False(deleted.IsActive);
        Assert.NotNull(deleted.ModifiedDate);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenActiveExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.Add(new Tour { Id = 1, TourName = "Exists", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenInactive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.Add(new Tour { Id = 1, TourName = "Inactive", IsActive = false });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.AddRange(
            new Tour { TourName = "Beach Paradise", Place = "Hawaii", IsActive = true },
            new Tour { TourName = "Mountain Trek", Place = "Nepal", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Beach");

        // Assert
        Assert.Single(result);
        Assert.Equal("Beach Paradise", result.First().TourName);
    }

    [Fact]
    public async Task SearchAsync_SearchesMultipleFields()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.AddRange(
            new Tour { TourName = "Tour A", Place = "Paris", Locations = "France", IsActive = true },
            new Tour { TourName = "Tour B", Place = "London", Locations = "UK", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.Single(result);
        Assert.Equal("Tour A", result.First().TourName);
    }
}
