using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagement.UnitTests.TourManagement.Infrastructure.Data;

public class TourManagementDbContextTests
{
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public TourManagementDbContextTests()
    {
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidOptions_CreatesInstance()
    {
        using var context = new TourManagementDbContext(_options);

        Assert.NotNull(context);
        Assert.NotNull(context.Tours);
        Assert.NotNull(context.Users);
        Assert.NotNull(context.Bookings);
    }

    [Fact]
    public async Task Tours_CanAddTour()
    {
        using var context = new TourManagementDbContext(_options);

        var tour = new Tour
        {
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 100m,
            Locations = "Locations",
            TourInfo = "Info",
            IsActive = true
        };

        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        Assert.Equal(1, await context.Tours.CountAsync());
    }

    [Fact]
    public async Task Users_CanAddUser()
    {
        using var context = new TourManagementDbContext(_options);

        var user = new UserInfo
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Gender = "Male",
            PasswordHash = "hash",
            DateOfBirth = DateTime.Now,
            Street = "Street",
            City = "City",
            State = "State",
            IsActive = true
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        Assert.Equal(1, await context.Users.CountAsync());
    }

    [Fact]
    public async Task Bookings_CanAddBooking()
    {
        using var context = new TourManagementDbContext(_options);

        var tour = new Tour
        {
            TourName = "Tour",
            Place = "Place",
            Days = 5,
            Price = 100m,
            Locations = "Loc",
            TourInfo = "Info",
            IsActive = true
        };
        var user = new UserInfo
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Gender = "Male",
            PasswordHash = "hash",
            DateOfBirth = DateTime.Now,
            Street = "St",
            City = "City",
            State = "State",
            IsActive = true
        };
        context.Tours.Add(tour);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var booking = new Booking
        {
            TourId = tour.Id,
            Email = user.Email,
            FirstName = "Test",
            TourName = "Tour",
            Place = "Place",
            BookingDate = DateTime.UtcNow,
            IsActive = true
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        Assert.Equal(1, await context.Bookings.CountAsync());
    }

    [Fact]
    public async Task OnModelCreating_AppliesConfigurations()
    {
        using var context = new TourManagementDbContext(_options);

        var tour = new Tour
        {
            TourName = "Test",
            Place = "Place",
            Days = 5,
            Price = 100m,
            Locations = "Loc",
            TourInfo = "Info",
            IsActive = true
        };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var savedTour = await context.Tours.FirstAsync();
        Assert.NotNull(savedTour);
        Assert.True(savedTour.IsActive);
    }

    [Fact]
    public async Task DbSets_AreNotNull()
    {
        using var context = new TourManagementDbContext(_options);

        Assert.NotNull(context.Tours);
        Assert.NotNull(context.Users);
        Assert.NotNull(context.Bookings);

        var toursCount = await context.Tours.CountAsync();
        var usersCount = await context.Users.CountAsync();
        var bookingsCount = await context.Bookings.CountAsync();

        Assert.Equal(0, toursCount);
        Assert.Equal(0, usersCount);
        Assert.Equal(0, bookingsCount);
    }
}
