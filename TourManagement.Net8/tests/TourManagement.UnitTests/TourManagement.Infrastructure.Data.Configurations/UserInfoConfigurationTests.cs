using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagement.UnitTests.TourManagement.Infrastructure.Data.Configurations;

public class UserInfoConfigurationTests
{
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public UserInfoConfigurationTests()
    {
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Configure_SetsTableName()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(UserInfo));

        Assert.NotNull(entityType);
        Assert.Equal("UserInfo", entityType.GetTableName());
    }

    [Fact]
    public void Configure_SetsPrimaryKey()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(UserInfo));
        var primaryKey = entityType!.FindPrimaryKey();

        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Email", primaryKey.Properties.First().Name);
    }

    [Fact]
    public void Configure_SetsRequiredProperties()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(UserInfo));

        var emailProperty = entityType!.FindProperty("Email");
        Assert.False(emailProperty.IsNullable);

        var firstNameProperty = entityType.FindProperty("FirstName");
        Assert.False(firstNameProperty.IsNullable);

        var lastNameProperty = entityType.FindProperty("LastName");
        Assert.False(lastNameProperty.IsNullable);
    }

    [Fact]
    public void Configure_SetsMaxLength()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(UserInfo));

        var emailProperty = entityType!.FindProperty("Email");
        Assert.Equal(50, emailProperty.GetMaxLength());

        var firstNameProperty = entityType.FindProperty("FirstName");
        Assert.Equal(50, firstNameProperty.GetMaxLength());

        var passwordHashProperty = entityType.FindProperty("PasswordHash");
        Assert.Equal(255, passwordHashProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_SetsUniqueIndex()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(UserInfo));
        var indexes = entityType!.GetIndexes();

        var emailIndex = indexes.FirstOrDefault(i => i.Properties.Any(p => p.Name == "Email"));
        Assert.NotNull(emailIndex);
        Assert.True(emailIndex.IsUnique);
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

        var savedUser = await context.Users.Include(u => u.Bookings).FirstAsync();
        Assert.Equal(2, savedUser.Bookings.Count);
    }

    [Fact]
    public void Configure_ColumnNames_AreSetCorrectly()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(UserInfo));

        var emailProperty = entityType!.FindProperty("Email");
        Assert.Equal("Email", emailProperty.GetColumnName());

        var passwordProperty = entityType.FindProperty("PasswordHash");
        Assert.Equal("Password", passwordProperty.GetColumnName());

        var dobProperty = entityType.FindProperty("DateOfBirth");
        Assert.Equal("dob", dobProperty.GetColumnName());
    }
}
