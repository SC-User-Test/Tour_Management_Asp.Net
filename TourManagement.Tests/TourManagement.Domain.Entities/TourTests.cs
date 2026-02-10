using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class TourTests
{
    [Fact]
    public void Tour_Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.Equal(0, tour.Id);
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(0, tour.Days);
        Assert.Equal(0, tour.Price);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Equal(string.Empty, tour.TourInfo);
        Assert.Null(tour.PictureFileName);
        Assert.False(tour.IsActive);
        Assert.Equal("System", tour.CreatedBy);
        Assert.Null(tour.ModifiedBy);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_SetProperties_StoresCorrectValues()
    {
        // Arrange
        var tour = new Tour();
        var createdDate = DateTime.UtcNow;
        var modifiedDate = DateTime.UtcNow.AddDays(1);

        // Act
        tour.Id = 1;
        tour.TourName = "Himalayan Adventure";
        tour.Place = "Nepal";
        tour.Days = 10;
        tour.Price = 2500.50m;
        tour.Locations = "Kathmandu, Pokhara, Everest Base Camp";
        tour.TourInfo = "Explore the majestic Himalayas";
        tour.PictureFileName = "himalaya.jpg";
        tour.CreatedDate = createdDate;
        tour.ModifiedDate = modifiedDate;
        tour.IsActive = true;
        tour.CreatedBy = "Admin";
        tour.ModifiedBy = "User";

        // Assert
        Assert.Equal(1, tour.Id);
        Assert.Equal("Himalayan Adventure", tour.TourName);
        Assert.Equal("Nepal", tour.Place);
        Assert.Equal(10, tour.Days);
        Assert.Equal(2500.50m, tour.Price);
        Assert.Equal("Kathmandu, Pokhara, Everest Base Camp", tour.Locations);
        Assert.Equal("Explore the majestic Himalayas", tour.TourInfo);
        Assert.Equal("himalaya.jpg", tour.PictureFileName);
        Assert.Equal(createdDate, tour.CreatedDate);
        Assert.Equal(modifiedDate, tour.ModifiedDate);
        Assert.True(tour.IsActive);
        Assert.Equal("Admin", tour.CreatedBy);
        Assert.Equal("User", tour.ModifiedBy);
    }

    [Fact]
    public void Tour_Bookings_CanAddAndRemove()
    {
        // Arrange
        var tour = new Tour();
        var booking = new Booking { Id = 1, TourId = 1 };

        // Act
        tour.Bookings.Add(booking);

        // Assert
        Assert.Single(tour.Bookings);
        Assert.Contains(booking, tour.Bookings);
    }

    [Fact]
    public void Tour_NegativePrice_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = -100;

        // Assert
        Assert.Equal(-100, tour.Price);
    }

    [Fact]
    public void Tour_ZeroDays_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 0;

        // Assert
        Assert.Equal(0, tour.Days);
    }

    [Fact]
    public void Tour_EmptyStrings_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourName = "";
        tour.Place = "";

        // Assert
        Assert.Equal("", tour.TourName);
        Assert.Equal("", tour.Place);
    }

    [Fact]
    public void Tour_NullPictureFileName_IsValid()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.PictureFileName = null;

        // Assert
        Assert.Null(tour.PictureFileName);
    }

    [Fact]
    public void Tour_NullModifiedBy_IsValid()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedBy = null;

        // Assert
        Assert.Null(tour.ModifiedBy);
    }
}
