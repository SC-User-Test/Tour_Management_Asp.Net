using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.UnitTests.TourManagement.Application.Services;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepo;
    private readonly Mock<ITourRepository> _mockTourRepo;
    private readonly Mock<IUserInfoRepository> _mockUserRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _mockBookingRepo = new Mock<IBookingRepository>();
        _mockTourRepo = new Mock<ITourRepository>();
        _mockUserRepo = new Mock<IUserInfoRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _service = new BookingService(_mockBookingRepo.Object, _mockTourRepo.Object, _mockUserRepo.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingService(null!, _mockTourRepo.Object, _mockUserRepo.Object, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullTourRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockBookingRepo.Object, null!, _mockUserRepo.Object, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullUserRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockBookingRepo.Object, _mockTourRepo.Object, null!, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullMapper_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockBookingRepo.Object, _mockTourRepo.Object, _mockUserRepo.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockBookingRepo.Object, _mockTourRepo.Object, _mockUserRepo.Object, _mockMapper.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsBookingDtos()
    {
        var bookings = new List<Booking> { new Booking { Id = 1 } };
        var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1 } };
        _mockBookingRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        var result = await _service.GetAllAsync();

        Assert.Equal(bookingDtos.Count, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsBookingDto()
    {
        var booking = new Booking { Id = 1 };
        var bookingDto = new BookingDto { Id = 1 };
        _mockBookingRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _mockMapper.Setup(m => m.Map<BookingDto>(booking)).Returns(bookingDto);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        _mockBookingRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserEmailAsync_ReturnsBookingsForUser()
    {
        var bookings = new List<Booking> { new Booking { Id = 1, Email = "test@test.com" } };
        var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1, Email = "test@test.com" } };
        _mockBookingRepo.Setup(r => r.GetByUserEmailAsync("test@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        var result = await _service.GetByUserEmailAsync("test@test.com");

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByTourIdAsync_ReturnsBookingsForTour()
    {
        var bookings = new List<Booking> { new Booking { Id = 1, TourId = 100 } };
        var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1, TourId = 100 } };
        _mockBookingRepo.Setup(r => r.GetByTourIdAsync(100, It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        var result = await _service.GetByTourIdAsync(100);

        Assert.Single(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesBooking()
    {
        var createDto = new BookingCreateDto { TourId = 1, Email = "test@test.com" };
        var tour = new Tour { Id = 1, TourName = "Tour", Place = "Place", Days = 5, Price = 100m, Locations = "Loc", TourInfo = "Info" };
        var user = new UserInfo { Email = "test@test.com", FirstName = "John", LastName = "Doe", Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        var booking = new Booking { Id = 1, TourId = 1, Email = "test@test.com", TourName = "Tour", Place = "Place", FirstName = "John" };
        var bookingDto = new BookingDto { Id = 1, TourId = 1, Email = "test@test.com" };

        _mockTourRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<Booking>(createDto)).Returns(booking);
        _mockBookingRepo.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _mockMapper.Setup(m => m.Map<BookingDto>(booking)).Returns(bookingDto);

        var result = await _service.CreateAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidTourId_ThrowsInvalidOperationException()
    {
        var createDto = new BookingCreateDto { TourId = 999, Email = "test@test.com" };
        _mockTourRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_WithInvalidEmail_ThrowsInvalidOperationException()
    {
        var createDto = new BookingCreateDto { TourId = 1, Email = "invalid@test.com" };
        var tour = new Tour { Id = 1, TourName = "Tour", Place = "Place", Days = 5, Price = 100m, Locations = "Loc", TourInfo = "Info" };
        _mockTourRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mockUserRepo.Setup(r => r.GetByEmailAsync("invalid@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesBooking()
    {
        var updateDto = new BookingUpdateDto { TourId = 1, Email = "test@test.com" };
        var existingBooking = new Booking { Id = 1, TourId = 1, Email = "test@test.com" };
        var tour = new Tour { Id = 1, TourName = "Tour", Place = "Place", Days = 5, Price = 100m, Locations = "Loc", TourInfo = "Info" };
        var user = new UserInfo { Email = "test@test.com", FirstName = "John", LastName = "Doe", Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "St", City = "City", State = "State" };
        var updatedBooking = new Booking { Id = 1, TourId = 1, Email = "test@test.com", TourName = "Tour", Place = "Place", FirstName = "John" };
        var bookingDto = new BookingDto { Id = 1, TourId = 1, Email = "test@test.com" };

        _mockBookingRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingBooking);
        _mockTourRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map(updateDto, existingBooking)).Returns(existingBooking);
        _mockBookingRepo.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedBooking);
        _mockMapper.Setup(m => m.Map<BookingDto>(updatedBooking)).Returns(bookingDto);

        var result = await _service.UpdateAsync(1, updateDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsInvalidOperationException()
    {
        var updateDto = new BookingUpdateDto { TourId = 1, Email = "test@test.com" };
        _mockBookingRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ReturnsTrue()
    {
        _mockBookingRepo.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockBookingRepo.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        _mockBookingRepo.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _service.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingBookings()
    {
        var bookings = new List<Booking> { new Booking { Id = 1, FirstName = "John" } };
        var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1, FirstName = "John" } };
        _mockBookingRepo.Setup(r => r.SearchAsync("John", It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        var result = await _service.SearchAsync("John");

        Assert.Single(result);
    }
}
