using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Tests;

public class TourManagementDbContextTests
{
    private readonly DbContextOptions<TourManagementDbContext> _dbOptions;

    public TourManagementDbContextTests()
    {
        _dbOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_InitializesContext()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_dbOptions);

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Tours);
        Assert.NotNull(context.UserInfos);
        Assert.NotNull(context.Bookings);
    }

    [Fact]
    public async Task Tours_CanAddAndRetrieve()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var tour = new Tour
        {
            TourName = "Test Tour",
            Place = "Test Place",
            IsActive = true
        };

        // Act
        context.Tours.Add(tour);
        await context.SaveChangesAsync();
        var retrieved = await context.Tours.FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Test Tour", retrieved.TourName);
    }

    [Fact]
    public async Task UserInfos_CanAddAndRetrieve()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var user = new UserInfo
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            IsActive = true
        };

        // Act
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();
        var retrieved = await context.UserInfos.FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("test@test.com", retrieved.Email);
    }

    [Fact]
    public async Task Bookings_CanAddAndRetrieve()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var tour = new Tour { TourName = "Test", IsActive = true };
        var user = new UserInfo { Email = "test@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var booking = new Booking
        {
            TourId = tour.Id,
            UserId = user.Id,
            CustomerName = "Customer",
            CustomerEmail = "customer@test.com",
            IsActive = true
        };

        // Act
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
        var retrieved = await context.Bookings.FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Customer", retrieved.CustomerName);
    }

    [Fact]
    public async Task Bookings_IncludeNavigationProperties()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var tour = new Tour { TourName = "Test Tour", IsActive = true };
        var user = new UserInfo { Email = "test@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var booking = new Booking
        {
            TourId = tour.Id,
            UserId = user.Id,
            CustomerName = "Test",
            IsActive = true
        };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var retrieved = await context.Bookings
            .Include(b => b.Tour)
            .Include(b => b.User)
            .FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(retrieved);
        Assert.NotNull(retrieved.Tour);
        Assert.NotNull(retrieved.User);
        Assert.Equal("Test Tour", retrieved.Tour.TourName);
        Assert.Equal("test@test.com", retrieved.User.Email);
    }

    [Fact]
    public async Task Tour_WithBookings_MaintainsRelationship()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var tour = new Tour { TourName = "Tour With Bookings", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var booking1 = new Booking { TourId = tour.Id, UserId = user.Id, IsActive = true };
        var booking2 = new Booking { TourId = tour.Id, UserId = user.Id, IsActive = true };
        context.Bookings.AddRange(booking1, booking2);
        await context.SaveChangesAsync();

        // Act
        var retrieved = await context.Tours
            .Include(t => t.Bookings)
            .FirstOrDefaultAsync(t => t.Id == tour.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(2, retrieved.Bookings.Count);
    }

    [Fact]
    public async Task UserInfo_WithBookings_MaintainsRelationship()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var tour = new Tour { TourName = "Test Tour", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var booking1 = new Booking { TourId = tour.Id, UserId = user.Id, IsActive = true };
        var booking2 = new Booking { TourId = tour.Id, UserId = user.Id, IsActive = true };
        context.Bookings.AddRange(booking1, booking2);
        await context.SaveChangesAsync();

        // Act
        var retrieved = await context.UserInfos
            .Include(u => u.Bookings)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(2, retrieved.Bookings.Count);
    }
}
