using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class BookingRepositoryTests
{
    private readonly Mock<ILogger<BookingRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _dbOptions;

    public BookingRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<BookingRepository>>();
        _dbOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        var user = new UserInfo { Id = 1, Email = "test@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.UserInfos.Add(user);
        context.Bookings.AddRange(
            new Booking { Id = 1, TourId = 1, UserId = 1, IsActive = true },
            new Booking { Id = 2, TourId = 1, UserId = 1, IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBooking_WhenExistsAndActive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        var user = new UserInfo { Id = 1, Email = "test@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.UserInfos.Add(user);
        context.Bookings.Add(new Booking
        {
            Id = 1,
            TourId = 1,
            UserId = 1,
            CustomerName = "Test Customer",
            IsActive = true
        });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Customer", result.CustomerName);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsBookingsForUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        var user1 = new UserInfo { Id = 1, Email = "user1@test.com", IsActive = true };
        var user2 = new UserInfo { Id = 2, Email = "user2@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.UserInfos.AddRange(user1, user2);
        context.Bookings.AddRange(
            new Booking { TourId = 1, UserId = 1, IsActive = true },
            new Booking { TourId = 1, UserId = 1, IsActive = true },
            new Booking { TourId = 1, UserId = 2, IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUserIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.UserId));
    }

    [Fact]
    public async Task GetByTourIdAsync_ReturnsBookingsForTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var tour1 = new Tour { Id = 1, TourName = "Tour 1", IsActive = true };
        var tour2 = new Tour { Id = 2, TourName = "Tour 2", IsActive = true };
        var user = new UserInfo { Id = 1, Email = "user@test.com", IsActive = true };
        context.Tours.AddRange(tour1, tour2);
        context.UserInfos.Add(user);
        context.Bookings.AddRange(
            new Booking { TourId = 1, UserId = 1, IsActive = true },
            new Booking { TourId = 1, UserId = 1, IsActive = true },
            new Booking { TourId = 2, UserId = 1, IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByTourIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.TourId));
    }

    [Fact]
    public async Task AddAsync_AddsAndReturnsBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        var user = new UserInfo { Id = 1, Email = "test@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();
        var booking = new Booking
        {
            TourId = 1,
            UserId = 1,
            CustomerName = "New Customer",
            IsActive = true
        };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("New Customer", result.CustomerName);
        Assert.Single(context.Bookings);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        var user = new UserInfo { Id = 1, Email = "test@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.UserInfos.Add(user);
        var booking = new Booking
        {
            TourId = 1,
            UserId = 1,
            CustomerName = "Original Name",
            IsActive = true
        };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        booking.CustomerName = "Updated Name";
        await repository.UpdateAsync(booking);

        // Assert
        var updated = await context.Bookings.FindAsync(booking.Id);
        Assert.Equal("Updated Name", updated?.CustomerName);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        var user = new UserInfo { Id = 1, Email = "test@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.UserInfos.Add(user);
        var booking = new Booking
        {
            Id = 1,
            TourId = 1,
            UserId = 1,
            IsActive = true
        };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(1);

        // Assert
        var deleted = await context.Bookings.FindAsync(1);
        Assert.NotNull(deleted);
        Assert.False(deleted.IsActive);
        Assert.NotNull(deleted.ModifiedDate);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenActiveExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        var user = new UserInfo { Id = 1, Email = "test@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.UserInfos.Add(user);
        context.Bookings.Add(new Booking
        {
            Id = 1,
            TourId = 1,
            UserId = 1,
            IsActive = true
        });
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
        var repository = new BookingRepository(context, _mockLogger.Object);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        var user = new UserInfo { Id = 1, Email = "test@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.UserInfos.Add(user);
        context.Bookings.Add(new Booking
        {
            Id = 1,
            TourId = 1,
            UserId = 1,
            IsActive = false
        });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }
}
