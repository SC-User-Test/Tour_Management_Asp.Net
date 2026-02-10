using Xunit;
using TourManagement.Domain.Entities;
using System;
using System.Linq;

namespace TourManagement.UnitTests.TourManagement.Domain.Entities;

public class TourTests
{
    [Fact]
    public void Tour_DefaultConstructor_InitializesProperties()
    {
        var tour = new Tour();

        Assert.Equal(0, tour.Id);
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(0, tour.Days);
        Assert.Equal(0m, tour.Price);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Equal(string.Empty, tour.TourInfo);
        Assert.Equal("System", tour.CreatedBy);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_SetProperties_ReturnsCorrectValues()
    {
        var tour = new Tour
        {
            Id = 1,
            TourName = "Paris Tour",
            Place = "Paris",
            Days = 7,
            Price = 1500.50m,
            Locations = "Eiffel Tower, Louvre",
            TourInfo = "Amazing tour of Paris",
            PictureFileName = "paris.jpg",
            IsActive = true
        };

        Assert.Equal(1, tour.Id);
        Assert.Equal("Paris Tour", tour.TourName);
        Assert.Equal("Paris", tour.Place);
        Assert.Equal(7, tour.Days);
        Assert.Equal(1500.50m, tour.Price);
        Assert.Equal("Eiffel Tower, Louvre", tour.Locations);
        Assert.Equal("Amazing tour of Paris", tour.TourInfo);
        Assert.Equal("paris.jpg", tour.PictureFileName);
        Assert.True(tour.IsActive);
    }

    [Fact]
    public void Tour_Bookings_InitializedAsEmptyList()
    {
        var tour = new Tour();

        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_Bookings_CanAddBookings()
    {
        var tour = new Tour { Id = 1, TourName = "Tour", Place = "Place", Days = 5, Price = 100m, Locations = "Loc", TourInfo = "Info" };
        var booking = new Booking { TourId = tour.Id, Email = "test@test.com", FirstName = "Test", TourName = "Tour", Place = "Place", BookingDate = DateTime.UtcNow };

        tour.Bookings.Add(booking);

        Assert.Single(tour.Bookings);
        Assert.Equal(booking, tour.Bookings.First());
    }

    [Fact]
    public void Tour_CreatedDate_CanBeSet()
    {
        var createdDate = DateTime.UtcNow;
        var tour = new Tour { CreatedDate = createdDate };

        Assert.Equal(createdDate, tour.CreatedDate);
    }

    [Fact]
    public void Tour_ModifiedDate_CanBeNull()
    {
        var tour = new Tour();

        Assert.Null(tour.ModifiedDate);
    }

    [Fact]
    public void Tour_ModifiedDate_CanBeSet()
    {
        var modifiedDate = DateTime.UtcNow;
        var tour = new Tour { ModifiedDate = modifiedDate };

        Assert.NotNull(tour.ModifiedDate);
        Assert.Equal(modifiedDate, tour.ModifiedDate);
    }

    [Fact]
    public void Tour_PictureFileName_CanBeNull()
    {
        var tour = new Tour();

        Assert.Null(tour.PictureFileName);
    }

    [Fact]
    public void Tour_ModifiedBy_CanBeNull()
    {
        var tour = new Tour();

        Assert.Null(tour.ModifiedBy);
    }

    [Fact]
    public void Tour_IsActive_DefaultsToFalse()
    {
        var tour = new Tour();

        Assert.False(tour.IsActive);
    }

    [Fact]
    public void Tour_Price_AcceptsDecimalValues()
    {
        var tour = new Tour { Price = 99.99m };

        Assert.Equal(99.99m, tour.Price);
    }

    [Fact]
    public void Tour_Days_AcceptsPositiveIntegers()
    {
        var tour = new Tour { Days = 14 };

        Assert.Equal(14, tour.Days);
    }
}
