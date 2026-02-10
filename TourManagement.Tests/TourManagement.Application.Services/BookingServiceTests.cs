using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockRepository;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _service = new BookingService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, CustomerName = "John Doe" },
            new Booking { Id = 2, CustomerName = "Jane Smith" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(bookings);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBooking_WhenExists()
    {
        // Arrange
        var booking = new Booking { Id = 1, CustomerName = "John Doe" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(booking);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John Doe", result.CustomerName);
        _mockRepository.Verify(r => r.GetByIdAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(999, default), Times.Once);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsBookingsForUser()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 5, CustomerName = "John" },
            new Booking { Id = 2, UserId = 5, CustomerName = "John" }
        };
        _mockRepository.Setup(r => r.GetByUserIdAsync(5, default)).ReturnsAsync(bookings);

        // Act
        var result = await _service.GetByUserIdAsync(5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(5, b.UserId));
        _mockRepository.Verify(r => r.GetByUserIdAsync(5, default), Times.Once);
    }

    [Fact]
    public async Task GetByTourIdAsync_ReturnsBookingsForTour()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 3 },
            new Booking { Id = 2, TourId = 3 }
        };
        _mockRepository.Setup(r => r.GetByTourIdAsync(3, default)).ReturnsAsync(bookings);

        // Act
        var result = await _service.GetByTourIdAsync(3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(3, b.TourId));
        _mockRepository.Verify(r => r.GetByTourIdAsync(3, default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsDatesStatusAndIsActive()
    {
        // Arrange
        var booking = new Booking
        {
            TourId = 1,
            UserId = 1,
            CustomerName = "Test User"
        };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), default))
            .ReturnsAsync((Booking b, CancellationToken ct) => b);

        // Act
        var result = await _service.CreateAsync(booking);

        // Assert
        Assert.True(result.IsActive);
        Assert.Equal("Pending", result.Status);
        Assert.NotEqual(default(DateTime), result.BookingDate);
        Assert.NotEqual(default(DateTime), result.CreatedDate);
        _mockRepository.Verify(r => r.AddAsync(It.Is<Booking>(b =>
            b.IsActive && b.Status == "Pending" &&
            b.BookingDate != default(DateTime) &&
            b.CreatedDate != default(DateTime)), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedDate()
    {
        // Arrange
        var booking = new Booking { Id = 1, CustomerName = "Updated" };
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(booking);

        // Assert
        Assert.NotEqual(default(DateTime), booking.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Booking>(b =>
            b.ModifiedDate != null), default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepository()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ThrowsException_PropagatesError()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(default)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
    }

    [Fact]
    public async Task CreateAsync_WithZeroPeople_StillCreates()
    {
        // Arrange
        var booking = new Booking
        {
            TourId = 1,
            UserId = 1,
            NumberOfPeople = 0
        };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), default))
            .ReturnsAsync((Booking b, CancellationToken ct) => b);

        // Act
        var result = await _service.CreateAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.NumberOfPeople);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), default), Times.Once);
    }
}
