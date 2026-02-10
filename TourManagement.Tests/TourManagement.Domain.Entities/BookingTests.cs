using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class BookingTests
{
    [Fact]
    public void Booking_Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal(0, booking.Id);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(0, booking.UserId);
        Assert.Equal(string.Empty, booking.CustomerName);
        Assert.Equal(string.Empty, booking.CustomerEmail);
        Assert.Null(booking.CustomerPhone);
        Assert.Equal(0, booking.NumberOfPeople);
        Assert.Equal("Pending", booking.Status);
        Assert.Null(booking.Notes);
        Assert.False(booking.IsActive);
    }

    [Fact]
    public void Booking_SetProperties_StoresCorrectValues()
    {
        // Arrange
        var booking = new Booking();
        var bookingDate = DateTime.UtcNow;
        var travelDate = DateTime.UtcNow.AddDays(30);
        var createdDate = DateTime.UtcNow;
        var modifiedDate = DateTime.UtcNow.AddDays(1);
        var tour = new Tour { Id = 1 };
        var user = new UserInfo { Id = 1 };

        // Act
        booking.Id = 1;
        booking.TourId = 1;
        booking.UserId = 1;
        booking.CustomerName = "Jane Smith";
        booking.CustomerEmail = "jane@example.com";
        booking.CustomerPhone = "+9876543210";
        booking.NumberOfPeople = 4;
        booking.BookingDate = bookingDate;
        booking.TravelDate = travelDate;
        booking.TotalAmount = 10000.00m;
        booking.Status = "Confirmed";
        booking.Notes = "Special dietary requirements";
        booking.CreatedDate = createdDate;
        booking.ModifiedDate = modifiedDate;
        booking.IsActive = true;
        booking.Tour = tour;
        booking.User = user;

        // Assert
        Assert.Equal(1, booking.Id);
        Assert.Equal(1, booking.TourId);
        Assert.Equal(1, booking.UserId);
        Assert.Equal("Jane Smith", booking.CustomerName);
        Assert.Equal("jane@example.com", booking.CustomerEmail);
        Assert.Equal("+9876543210", booking.CustomerPhone);
        Assert.Equal(4, booking.NumberOfPeople);
        Assert.Equal(bookingDate, booking.BookingDate);
        Assert.Equal(travelDate, booking.TravelDate);
        Assert.Equal(10000.00m, booking.TotalAmount);
        Assert.Equal("Confirmed", booking.Status);
        Assert.Equal("Special dietary requirements", booking.Notes);
        Assert.Equal(createdDate, booking.CreatedDate);
        Assert.Equal(modifiedDate, booking.ModifiedDate);
        Assert.True(booking.IsActive);
        Assert.Equal(tour, booking.Tour);
        Assert.Equal(user, booking.User);
    }

    [Fact]
    public void Booking_DefaultStatus_IsPending()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal("Pending", booking.Status);
    }

    [Fact]
    public void Booking_NullCustomerPhone_IsValid()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.CustomerPhone = null;

        // Assert
        Assert.Null(booking.CustomerPhone);
    }

    [Fact]
    public void Booking_NullNotes_IsValid()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Notes = null;

        // Assert
        Assert.Null(booking.Notes);
    }

    [Fact]
    public void Booking_ZeroNumberOfPeople_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = 0;

        // Assert
        Assert.Equal(0, booking.NumberOfPeople);
    }

    [Fact]
    public void Booking_NegativeTotalAmount_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalAmount = -500;

        // Assert
        Assert.Equal(-500, booking.TotalAmount);
    }

    [Fact]
    public void Booking_StatusChange_UpdatesCorrectly()
    {
        // Arrange
        var booking = new Booking { Status = "Pending" };

        // Act
        booking.Status = "Cancelled";

        // Assert
        Assert.Equal("Cancelled", booking.Status);
    }

    [Fact]
    public void Booking_NavigationProperties_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { Id = 5, TourName = "Beach Paradise" };
        var user = new UserInfo { Id = 3, Email = "user@example.com" };

        // Act
        booking.Tour = tour;
        booking.User = user;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.NotNull(booking.User);
        Assert.Equal(5, booking.Tour.Id);
        Assert.Equal(3, booking.User.Id);
    }
}
