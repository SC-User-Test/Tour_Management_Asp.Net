using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Xunit;

namespace TourManagement.IntegrationTests.Repositories;

public class TourRepositoryTests
{
    private readonly TourManagementDbContext _context;
    private readonly TourRepository _repository;

    public TourRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TourManagementDbContext(options);
        var mockLogger = new Mock<ILogger<TourRepository>>();
        _repository = new TourRepository(_context, mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveTours()
    {
        var tour = new Tour
        {
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 1000,
            Locations = "Test Locations",
            TourInfo = "Test Info",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        await _context.Tours.AddAsync(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().NotBeEmpty();
    }
}
