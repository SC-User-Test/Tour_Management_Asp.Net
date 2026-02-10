using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class UserInfoTests
{
    [Fact]
    public void UserInfo_Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var userInfo = new UserInfo();

        // Assert
        Assert.Equal(0, userInfo.Id);
        Assert.Equal(string.Empty, userInfo.Email);
        Assert.Equal(string.Empty, userInfo.PasswordHash);
        Assert.Equal(string.Empty, userInfo.FirstName);
        Assert.Equal(string.Empty, userInfo.LastName);
        Assert.Null(userInfo.Phone);
        Assert.Null(userInfo.Address);
        Assert.False(userInfo.IsActive);
        Assert.Null(userInfo.ProfilePictureFileName);
        Assert.NotNull(userInfo.Bookings);
        Assert.Empty(userInfo.Bookings);
    }

    [Fact]
    public void UserInfo_SetProperties_StoresCorrectValues()
    {
        // Arrange
        var userInfo = new UserInfo();
        var createdDate = DateTime.UtcNow;
        var modifiedDate = DateTime.UtcNow.AddDays(1);

        // Act
        userInfo.Id = 1;
        userInfo.Email = "test@example.com";
        userInfo.PasswordHash = "hashedpassword123";
        userInfo.FirstName = "John";
        userInfo.LastName = "Doe";
        userInfo.Phone = "+1234567890";
        userInfo.Address = "123 Main St";
        userInfo.CreatedDate = createdDate;
        userInfo.ModifiedDate = modifiedDate;
        userInfo.IsActive = true;
        userInfo.ProfilePictureFileName = "profile.jpg";

        // Assert
        Assert.Equal(1, userInfo.Id);
        Assert.Equal("test@example.com", userInfo.Email);
        Assert.Equal("hashedpassword123", userInfo.PasswordHash);
        Assert.Equal("John", userInfo.FirstName);
        Assert.Equal("Doe", userInfo.LastName);
        Assert.Equal("+1234567890", userInfo.Phone);
        Assert.Equal("123 Main St", userInfo.Address);
        Assert.Equal(createdDate, userInfo.CreatedDate);
        Assert.Equal(modifiedDate, userInfo.ModifiedDate);
        Assert.True(userInfo.IsActive);
        Assert.Equal("profile.jpg", userInfo.ProfilePictureFileName);
    }

    [Fact]
    public void UserInfo_Bookings_CanAddAndRemove()
    {
        // Arrange
        var userInfo = new UserInfo();
        var booking = new Booking { Id = 1, UserId = 1 };

        // Act
        userInfo.Bookings.Add(booking);

        // Assert
        Assert.Single(userInfo.Bookings);
        Assert.Contains(booking, userInfo.Bookings);
    }

    [Fact]
    public void UserInfo_NullPhone_IsValid()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Phone = null;

        // Assert
        Assert.Null(userInfo.Phone);
    }

    [Fact]
    public void UserInfo_NullAddress_IsValid()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Address = null;

        // Assert
        Assert.Null(userInfo.Address);
    }

    [Fact]
    public void UserInfo_NullProfilePictureFileName_IsValid()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.ProfilePictureFileName = null;

        // Assert
        Assert.Null(userInfo.ProfilePictureFileName);
    }

    [Fact]
    public void UserInfo_EmptyEmail_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Email = "";

        // Assert
        Assert.Equal("", userInfo.Email);
    }

    [Fact]
    public void UserInfo_EmptyPasswordHash_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.PasswordHash = "";

        // Assert
        Assert.Equal("", userInfo.PasswordHash);
    }

    [Fact]
    public void UserInfo_MultipleBookings_CanBeAdded()
    {
        // Arrange
        var userInfo = new UserInfo();
        var booking1 = new Booking { Id = 1, UserId = 1 };
        var booking2 = new Booking { Id = 2, UserId = 1 };

        // Act
        userInfo.Bookings.Add(booking1);
        userInfo.Bookings.Add(booking2);

        // Assert
        Assert.Equal(2, userInfo.Bookings.Count);
        Assert.Contains(booking1, userInfo.Bookings);
        Assert.Contains(booking2, userInfo.Bookings);
    }
}
