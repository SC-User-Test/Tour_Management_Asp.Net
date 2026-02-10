using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class TourServiceTests
{
    private readonly Mock<ITourRepository> _mockRepository;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _service;

    public TourServiceTests()
    {
        _mockRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<TourService>>();
        _service = new TourService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Tour 1" },
            new Tour { Id = 2, TourName = "Tour 2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(tours);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTour_WhenExists()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(tour);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Tour", result.TourName);
        _mockRepository.Verify(r => r.GetByIdAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(999, default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsDatesAndIsActive()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour", Price = 1000 };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), default))
            .ReturnsAsync((Tour t, CancellationToken ct) => t);

        // Act
        var result = await _service.CreateAsync(tour);

        // Assert
        Assert.True(result.IsActive);
        Assert.NotEqual(default(DateTime), result.CreatedDate);
        _mockRepository.Verify(r => r.AddAsync(It.Is<Tour>(t =>
            t.IsActive && t.CreatedDate != default(DateTime)), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedDate()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Updated Tour" };
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(tour);

        // Assert
        Assert.NotEqual(default(DateTime), tour.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Tour>(t =>
            t.ModifiedDate != null), default), Times.Once);
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
    public async Task SearchAsync_ReturnsMatchingTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Beach Tour" }
        };
        _mockRepository.Setup(r => r.SearchAsync("Beach", default)).ReturnsAsync(tours);

        // Act
        var result = await _service.SearchAsync("Beach");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Beach Tour", result.First().TourName);
        _mockRepository.Verify(r => r.SearchAsync("Beach", default), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ThrowsException_LogsError()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(default)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
    }

    [Fact]
    public async Task CreateAsync_WithNullTourName_StillCreates()
    {
        // Arrange
        var tour = new Tour { TourName = null! };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), default))
            .ReturnsAsync((Tour t, CancellationToken ct) => t);

        // Act
        var result = await _service.CreateAsync(tour);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), default), Times.Once);
    }
}
