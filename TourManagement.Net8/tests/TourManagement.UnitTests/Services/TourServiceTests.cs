using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.DTOs;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Services;

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
    public async Task GetAllAsync_ShouldReturnAllTours()
    {
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Tour 1", IsActive = true },
            new Tour { Id = 2, TourName = "Tour 2", IsActive = true }
        };

        var tourDtos = new List<TourDto>
        {
            new TourDto { Id = 1, TourName = "Tour 1" },
            new TourDto { Id = 2, TourName = "Tour 2" }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours))
            .Returns(tourDtos);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(tourDtos);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        var tour = new Tour { Id = 1, TourName = "Tour 1", IsActive = true };
        var tourDto = new TourDto { Id = 1, TourName = "Tour 1" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        _mockMapper.Setup(m => m.Map<TourDto>(tour))
            .Returns(tourDto);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(tourDto);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldCreateTour()
    {
        var createDto = new TourCreateDto { TourName = "New Tour", Place = "New Place", Days = 5, Price = 1000 };
        var tour = new Tour { Id = 1, TourName = "New Tour", Place = "New Place", Days = 5, Price = 1000, IsActive = true };
        var tourDto = new TourDto { Id = 1, TourName = "New Tour" };

        _mockMapper.Setup(m => m.Map<Tour>(createDto))
            .Returns(tour);

        _mockRepository.Setup(r => r.AddAsync(tour, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        _mockMapper.Setup(m => m.Map<TourDto>(tour))
            .Returns(tourDto);

        var result = await _service.CreateAsync(createDto);

        result.Should().NotBeNull();
        result.TourName.Should().Be("New Tour");
    }
}
