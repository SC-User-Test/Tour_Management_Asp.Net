using Xunit;
using TourManagement.Domain.Entities;
using System;

namespace TourManagement.UnitTests.TourManagement.Domain.Entities;

public class BookingTests
{
    [Fact]
    public void Booking_DefaultConstructor_InitializesProperties()
    {
        var booking = new Booking();

        Assert.Equal(0, booking.Id);
        Assert.Equal(string.Empty, booking.TourName);
        Assert.Equal(string.Empty, booking.Place);
        Assert.Equal(string.Empty, booking.Email);
        Assert.Equal(string.Empty, booking.FirstName);
        Assert.Equal("System", booking.CreatedBy);
    }

    [Fact]
    public void Booking_SetProperties_ReturnsCorrectValues()
    {
        var booking = new Booking
        {
            Id = 1,
            TourId = 100,
            TourName = "Paris Tour",
            Place = "Paris",
            Email = "test@test.com",
            FirstName = "John",
            BookingDate = DateTime.UtcNow,
            IsActive = true
        };

        Assert.Equal(1, booking.Id);
        Assert.Equal(100, booking.TourId);
        Assert.Equal("Paris Tour", booking.TourName);
        Assert.Equal("Paris", booking.Place);
        Assert.Equal("test@test.com", booking.Email);
        Assert.Equal("John", booking.FirstName);
        Assert.True(booking.IsActive);
    }

    [Fact]
    public void Booking_NavigationProperties_CanBeSet()
    {
        var tour = new Tour { Id = 1, TourName = "Tour", Place = "Place", Days = 5, Price = 100m, Locations = "Loc", TourInfo = "Info" };
        var user = new UserInfo { Email = "test@test.com", FirstName = "Test", LastName = "User", Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };

        var booking = new Booking
        {
            TourId = tour.Id,
            Email = user.Email,
            Tour = tour,
            User = user
        };

        Assert.NotNull(booking.Tour);
        Assert.NotNull(booking.User);
        Assert.Equal(tour.Id, booking.Tour.Id);
        Assert.Equal(user.Email, booking.User.Email);
    }

    [Fact]
    public void Booking_CreatedDate_CanBeSet()
    {
        var createdDate = DateTime.UtcNow;
        var booking = new Booking { CreatedDate = createdDate };

        Assert.Equal(createdDate, booking.CreatedDate);
    }

    [Fact]
    public void Booking_ModifiedDate_CanBeNull()
    {
        var booking = new Booking();

        Assert.Null(booking.ModifiedDate);
    }

    [Fact]
    public void Booking_ModifiedDate_CanBeSet()
    {
        var modifiedDate = DateTime.UtcNow;
        var booking = new Booking { ModifiedDate = modifiedDate };

        Assert.NotNull(booking.ModifiedDate);
        Assert.Equal(modifiedDate, booking.ModifiedDate);
    }

    [Fact]
    public void Booking_ModifiedBy_CanBeNull()
    {
        var booking = new Booking();

        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void Booking_ModifiedBy_CanBeSet()
    {
        var booking = new Booking { ModifiedBy = "Admin" };

        Assert.Equal("Admin", booking.ModifiedBy);
    }

    [Fact]
    public void Booking_IsActive_DefaultsToFalse()
    {
        var booking = new Booking();

        Assert.False(booking.IsActive);
    }
}
