using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagement.UnitTests.TourManagement.Infrastructure.Data.Configurations;

public class TourConfigurationTests
{
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public TourConfigurationTests()
    {
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Configure_SetsTableName()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        Assert.NotNull(entityType);
        Assert.Equal("Tour", entityType.GetTableName());
    }

    [Fact]
    public void Configure_SetsPrimaryKey()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var primaryKey = entityType!.FindPrimaryKey();

        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties.First().Name);
    }

    [Fact]
    public void Configure_SetsRequiredProperties()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        var tourNameProperty = entityType!.FindProperty("TourName");
        Assert.NotNull(tourNameProperty);
        Assert.False(tourNameProperty.IsNullable);

        var placeProperty = entityType.FindProperty("Place");
        Assert.NotNull(placeProperty);
        Assert.False(placeProperty.IsNullable);
    }

    [Fact]
    public void Configure_SetsMaxLength()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        var tourNameProperty = entityType!.FindProperty("TourName");
        Assert.Equal(20, tourNameProperty.GetMaxLength());

        var placeProperty = entityType.FindProperty("Place");
        Assert.Equal(20, placeProperty.GetMaxLength());

        var locationsProperty = entityType.FindProperty("Locations");
        Assert.Equal(100, locationsProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_SetsIndexes()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var indexes = entityType!.GetIndexes();

        Assert.NotNull(indexes);
        Assert.True(indexes.Any(i => i.Properties.Any(p => p.Name == "TourName")));
        Assert.True(indexes.Any(i => i.Properties.Any(p => p.Name == "Place")));
    }

    [Fact]
    public async Task Configure_HasManyBookings_IsConfigured()
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

        var booking1 = new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Test1", TourName = "Tour", Place = "Place", BookingDate = DateTime.UtcNow, IsActive = true };
        var booking2 = new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Test2", TourName = "Tour", Place = "Place", BookingDate = DateTime.UtcNow, IsActive = true };
        context.Bookings.AddRange(booking1, booking2);
        await context.SaveChangesAsync();

        var savedTour = await context.Tours.Include(t => t.Bookings).FirstAsync();
        Assert.Equal(2, savedTour.Bookings.Count);
    }

    [Fact]
    public void Configure_PriceColumnType_IsDecimal()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var priceProperty = entityType!.FindProperty("Price");

        Assert.NotNull(priceProperty);
        Assert.Equal("decimal(18,2)", priceProperty.GetColumnType());
    }
}
