using Xunit;
using TourManagement.Domain.Entities;
using System;
using System.Linq;

namespace TourManagement.UnitTests.TourManagement.Domain.Entities;

public class UserInfoTests
{
    [Fact]
    public void UserInfo_DefaultConstructor_InitializesProperties()
    {
        var user = new UserInfo();

        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
        Assert.Equal(string.Empty, user.Gender);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(string.Empty, user.Street);
        Assert.Equal(string.Empty, user.City);
        Assert.Equal(string.Empty, user.State);
        Assert.Equal("System", user.CreatedBy);
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void UserInfo_SetProperties_ReturnsCorrectValues()
    {
        var dateOfBirth = new DateTime(1990, 1, 1);
        var user = new UserInfo
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            PasswordHash = "hashedpassword123",
            DateOfBirth = dateOfBirth,
            Street = "123 Main St",
            City = "New York",
            State = "NY",
            IsActive = true
        };

        Assert.Equal("test@test.com", user.Email);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("Male", user.Gender);
        Assert.Equal("hashedpassword123", user.PasswordHash);
        Assert.Equal(dateOfBirth, user.DateOfBirth);
        Assert.Equal("123 Main St", user.Street);
        Assert.Equal("New York", user.City);
        Assert.Equal("NY", user.State);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void UserInfo_Bookings_InitializedAsEmptyList()
    {
        var user = new UserInfo();

        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void UserInfo_Bookings_CanAddBookings()
    {
        var user = new UserInfo { Email = "test@test.com", FirstName = "Test", LastName = "User", Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        var booking = new Booking { Email = user.Email, FirstName = "Test", TourName = "Tour", Place = "Place", BookingDate = DateTime.UtcNow };

        user.Bookings.Add(booking);

        Assert.Single(user.Bookings);
        Assert.Equal(booking, user.Bookings.First());
    }

    [Fact]
    public void UserInfo_CreatedDate_CanBeSet()
    {
        var createdDate = DateTime.UtcNow;
        var user = new UserInfo { CreatedDate = createdDate };

        Assert.Equal(createdDate, user.CreatedDate);
    }

    [Fact]
    public void UserInfo_ModifiedDate_CanBeNull()
    {
        var user = new UserInfo();

        Assert.Null(user.ModifiedDate);
    }

    [Fact]
    public void UserInfo_ModifiedDate_CanBeSet()
    {
        var modifiedDate = DateTime.UtcNow;
        var user = new UserInfo { ModifiedDate = modifiedDate };

        Assert.NotNull(user.ModifiedDate);
        Assert.Equal(modifiedDate, user.ModifiedDate);
    }

    [Fact]
    public void UserInfo_ModifiedBy_CanBeNull()
    {
        var user = new UserInfo();

        Assert.Null(user.ModifiedBy);
    }

    [Fact]
    public void UserInfo_ModifiedBy_CanBeSet()
    {
        var user = new UserInfo { ModifiedBy = "Admin" };

        Assert.Equal("Admin", user.ModifiedBy);
    }

    [Fact]
    public void UserInfo_IsActive_DefaultsToFalse()
    {
        var user = new UserInfo();

        Assert.False(user.IsActive);
    }

    [Fact]
    public void UserInfo_DateOfBirth_CanBeSet()
    {
        var dob = new DateTime(1985, 5, 15);
        var user = new UserInfo { DateOfBirth = dob };

        Assert.Equal(dob, user.DateOfBirth);
    }

    [Fact]
    public void UserInfo_Gender_AcceptsDifferentValues()
    {
        var user1 = new UserInfo { Gender = "Male" };
        var user2 = new UserInfo { Gender = "Female" };
        var user3 = new UserInfo { Gender = "Other" };

        Assert.Equal("Male", user1.Gender);
        Assert.Equal("Female", user2.Gender);
        Assert.Equal("Other", user3.Gender);
    }
}
