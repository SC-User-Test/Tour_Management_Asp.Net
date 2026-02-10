using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.UnitTests.TourManagement.Infrastructure.Repositories;

public class BookingRepositoryTests
{
    private readonly Mock<ILogger<BookingRepository>> _mockLogger;
    private readonly TourManagementDbContext _context;
    private readonly BookingRepository _repository;

    public BookingRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new TourManagementDbContext(options);
        _mockLogger = new Mock<ILogger<BookingRepository>>();
        _repository = new BookingRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new BookingRepository(null!, _mockLogger.Object));
        Assert.Equal("context", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new BookingRepository(_context, null!));
        Assert.Equal("logger", exception.ParamName);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveBookings()
    {
        var tour = new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Tours.AddAsync(tour);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var bookings = new List<Booking>
        {
            new Booking { TourId = tour.Id, Email = user.Email, FirstName = "John", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true },
            new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Jane", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true },
            new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Bob", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = false }
        };
        await _context.Bookings.AddRangeAsync(bookings);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.All(result, booking => Assert.True(booking.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_ReturnsList()
    {
        var tour = new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Tours.AddAsync(tour);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var booking = new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Test", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true };
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsBooking()
    {
        var tour = new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Tours.AddAsync(tour);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var booking = new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Test", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true };
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(booking.Id);

        Assert.NotNull(result);
        Assert.Equal(booking.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserEmailAsync_ReturnsBookingsForUser()
    {
        var tour = new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Tours.AddAsync(tour);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var bookings = new List<Booking>
        {
            new Booking { TourId = tour.Id, Email = "user@test.com", FirstName = "Test1", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true },
            new Booking { TourId = tour.Id, Email = "user@test.com", FirstName = "Test2", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true }
        };
        await _context.Bookings.AddRangeAsync(bookings);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByUserEmailAsync("user@test.com");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByTourIdAsync_ReturnsBookingsForTour()
    {
        var tour = new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Tours.AddAsync(tour);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var bookings = new List<Booking>
        {
            new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Test1", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true },
            new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Test2", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true }
        };
        await _context.Bookings.AddRangeAsync(bookings);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByTourIdAsync(tour.Id);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AddAsync_WithValidBooking_AddsBooking()
    {
        var tour = new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Tours.AddAsync(tour);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var booking = new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Test", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true };
        var result = await _repository.AddAsync(booking);

        Assert.NotNull(result);
        Assert.Single(_context.Bookings);
    }

    [Fact]
    public async Task UpdateAsync_WithValidBooking_UpdatesBooking()
    {
        var tour = new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Tours.AddAsync(tour);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var booking = new Booking { TourId = tour.Id, Email = user.Email, FirstName = "OldName", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true };
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
        _context.Entry(booking).State = EntityState.Detached;

        booking.FirstName = "NewName";
        var result = await _repository.UpdateAsync(booking);

        Assert.Equal("NewName", result.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_SoftDeletesBooking()
    {
        var tour = new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Tours.AddAsync(tour);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var booking = new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Test", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true };
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var result = await _repository.DeleteAsync(booking.Id);

        Assert.True(result);
        var deletedBooking = await _context.Bookings.FindAsync(booking.Id);
        Assert.False(deletedBooking!.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        var result = await _repository.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingBooking_ReturnsTrue()
    {
        var tour = new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Tours.AddAsync(tour);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var booking = new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Test", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true };
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(booking.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingBooking_ReturnsFalse()
    {
        var result = await _repository.ExistsAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithEmptyTerm_ReturnsAllBookings()
    {
        var tour = new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Tours.AddAsync(tour);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var bookings = new List<Booking>
        {
            new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Test1", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true },
            new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Test2", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true }
        };
        await _context.Bookings.AddRangeAsync(bookings);
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ReturnsMatchingBookings()
    {
        var tour = new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true };
        var user = new UserInfo { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        await _context.Tours.AddAsync(tour);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var bookings = new List<Booking>
        {
            new Booking { TourId = tour.Id, Email = user.Email, FirstName = "John", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true },
            new Booking { TourId = tour.Id, Email = user.Email, FirstName = "Jane", TourName = "Tour1", Place = "Place1", BookingDate = DateTime.UtcNow, IsActive = true }
        };
        await _context.Bookings.AddRangeAsync(bookings);
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("John");

        Assert.Single(result);
        Assert.Equal("John", result.First().FirstName);
    }
}
