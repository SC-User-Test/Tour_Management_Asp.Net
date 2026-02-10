using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Data.Configurations;
using TourManagement.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagement.UnitTests.TourManagement.Infrastructure.Data.Configurations;

public class BookingConfigurationTests
{
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public BookingConfigurationTests()
    {
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Configure_SetsTableName()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        Assert.NotNull(entityType);
        Assert.Equal("booking", entityType.GetTableName());
    }

    [Fact]
    public void Configure_SetsPrimaryKey()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var primaryKey = entityType!.FindPrimaryKey();

        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties.First().Name);
    }

    [Fact]
    public void Configure_SetsRequiredProperties()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        var tourIdProperty = entityType!.FindProperty("TourId");
        Assert.NotNull(tourIdProperty);
        Assert.False(tourIdProperty.IsNullable);
    }

    [Fact]
    public void Configure_SetsMaxLength()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        var tourNameProperty = entityType!.FindProperty("TourName");
        Assert.NotNull(tourNameProperty);
        Assert.Equal(50, tourNameProperty.GetMaxLength());

        var placeProperty = entityType.FindProperty("Place");
        Assert.NotNull(placeProperty);
        Assert.Equal(50, placeProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_SetsIndexes()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var indexes = entityType!.GetIndexes();

        Assert.NotNull(indexes);
        Assert.True(indexes.Any(i => i.Properties.Any(p => p.Name == "Email")));
        Assert.True(indexes.Any(i => i.Properties.Any(p => p.Name == "TourId")));
    }

    [Fact]
    public async Task Configure_ForeignKeyToTour_IsConfigured()
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

        var savedBooking = await context.Bookings.Include(b => b.Tour).FirstAsync();
        Assert.NotNull(savedBooking.Tour);
        Assert.Equal(tour.Id, savedBooking.Tour.Id);
    }

    [Fact]
    public async Task Configure_ForeignKeyToUser_IsConfigured()
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

        var savedBooking = await context.Bookings.Include(b => b.User).FirstAsync();
        Assert.NotNull(savedBooking.User);
        Assert.Equal(user.Email, savedBooking.User.Email);
    }
}
