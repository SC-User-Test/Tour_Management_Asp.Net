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

public class TourServiceTests
{
    private readonly Mock<ITourRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _service;

    public TourServiceTests()
    {
        _mockRepository = new Mock<ITourRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<TourService>>();
        _service = new TourService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TourService(null!, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullMapper_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TourService(_mockRepository.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TourService(_mockRepository.Object, _mockMapper.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsTourDtos()
    {
        var tours = new List<Tour> { new Tour { Id = 1, TourName = "Tour1" } };
        var tourDtos = new List<TourDto> { new TourDto { Id = 1, TourName = "Tour1" } };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        var result = await _service.GetAllAsync();

        Assert.Equal(tourDtos.Count, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTourDto()
    {
        var tour = new Tour { Id = 1, TourName = "Tour1" };
        var tourDto = new TourDto { Id = 1, TourName = "Tour1" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mockMapper.Setup(m => m.Map<TourDto>(tour)).Returns(tourDto);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesTour()
    {
        var createDto = new TourCreateDto { TourName = "New Tour", Place = "Paris", Days = 5, Price = 100m, Locations = "Loc", TourInfo = "Info" };
        var tour = new Tour { Id = 1, TourName = "New Tour" };
        var tourDto = new TourDto { Id = 1, TourName = "New Tour" };
        _mockMapper.Setup(m => m.Map<Tour>(createDto)).Returns(tour);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mockMapper.Setup(m => m.Map<TourDto>(tour)).Returns(tourDto);

        var result = await _service.CreateAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyTourName_ThrowsArgumentException()
    {
        var createDto = new TourCreateDto { TourName = "", Place = "Paris" };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_WithWhitespaceTourName_ThrowsArgumentException()
    {
        var createDto = new TourCreateDto { TourName = "   ", Place = "Paris" };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesTour()
    {
        var updateDto = new TourUpdateDto { TourName = "Updated Tour", Place = "London", Days = 7, Price = 200m, Locations = "Loc", TourInfo = "Info" };
        var existingTour = new Tour { Id = 1, TourName = "Old Tour" };
        var updatedTour = new Tour { Id = 1, TourName = "Updated Tour" };
        var tourDto = new TourDto { Id = 1, TourName = "Updated Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingTour);
        _mockMapper.Setup(m => m.Map(updateDto, existingTour)).Returns(existingTour);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedTour);
        _mockMapper.Setup(m => m.Map<TourDto>(updatedTour)).Returns(tourDto);

        var result = await _service.UpdateAsync(1, updateDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsInvalidOperationException()
    {
        var updateDto = new TourUpdateDto { TourName = "Updated Tour", Place = "London" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ReturnsTrue()
    {
        _mockRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _service.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingTours()
    {
        var tours = new List<Tour> { new Tour { Id = 1, TourName = "Paris Tour" } };
        var tourDtos = new List<TourDto> { new TourDto { Id = 1, TourName = "Paris Tour" } };
        _mockRepository.Setup(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        var result = await _service.SearchAsync("Paris");

        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_ReturnsList()
    {
        var tours = new List<Tour> { new Tour { Id = 1 } };
        var tourDtos = new List<TourDto> { new TourDto { Id = 1 } };
        _mockRepository.Setup(r => r.GetAllAsync(CancellationToken.None)).ReturnsAsync(tours);
        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        var result = await _service.GetAllAsync(CancellationToken.None);

        Assert.Single(result);
    }
}
