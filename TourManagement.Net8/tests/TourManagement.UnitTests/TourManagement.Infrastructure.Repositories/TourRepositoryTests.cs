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

public class TourRepositoryTests
{
    private readonly Mock<ILogger<TourRepository>> _mockLogger;
    private readonly TourManagementDbContext _context;
    private readonly TourRepository _repository;

    public TourRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new TourManagementDbContext(options);
        _mockLogger = new Mock<ILogger<TourRepository>>();
        _repository = new TourRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new TourRepository(null!, _mockLogger.Object));
        Assert.Equal("context", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new TourRepository(_context, null!));
        Assert.Equal("logger", exception.ParamName);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveTours()
    {
        var tours = new List<Tour>
        {
            new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true },
            new Tour { TourName = "Tour2", Place = "Place2", Days = 7, Price = 200m, Locations = "Loc2", TourInfo = "Info2", IsActive = true },
            new Tour { TourName = "Tour3", Place = "Place3", Days = 3, Price = 150m, Locations = "Loc3", TourInfo = "Info3", IsActive = false }
        };
        await _context.Tours.AddRangeAsync(tours);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.All(result, tour => Assert.True(tour.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_ReturnsList()
    {
        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 100m, Locations = "Locations", TourInfo = "Info", IsActive = true };
        await _context.Tours.AddAsync(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTour()
    {
        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 100m, Locations = "Locations", TourInfo = "Info", IsActive = true };
        await _context.Tours.AddAsync(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(tour.Id);

        Assert.NotNull(result);
        Assert.Equal(tour.Id, result.Id);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveTour_ReturnsNull()
    {
        var tour = new Tour { TourName = "Inactive Tour", Place = "Place", Days = 5, Price = 100m, Locations = "Locations", TourInfo = "Info", IsActive = false };
        await _context.Tours.AddAsync(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(tour.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WithValidTour_AddsTour()
    {
        var tour = new Tour { TourName = "New Tour", Place = "New Place", Days = 5, Price = 100m, Locations = "Locations", TourInfo = "Info", IsActive = true };

        var result = await _repository.AddAsync(tour);

        Assert.NotNull(result);
        Assert.Equal("New Tour", result.TourName);
        Assert.Single(_context.Tours);
    }

    [Fact]
    public async Task UpdateAsync_WithValidTour_UpdatesTour()
    {
        var tour = new Tour { TourName = "Old Name", Place = "Old Place", Days = 5, Price = 100m, Locations = "Locations", TourInfo = "Info", IsActive = true };
        await _context.Tours.AddAsync(tour);
        await _context.SaveChangesAsync();
        _context.Entry(tour).State = EntityState.Detached;

        tour.TourName = "New Name";
        var result = await _repository.UpdateAsync(tour);

        Assert.Equal("New Name", result.TourName);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_SoftDeletesTour()
    {
        var tour = new Tour { TourName = "Delete Tour", Place = "Place", Days = 5, Price = 100m, Locations = "Locations", TourInfo = "Info", IsActive = true };
        await _context.Tours.AddAsync(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.DeleteAsync(tour.Id);

        Assert.True(result);
        var deletedTour = await _context.Tours.FindAsync(tour.Id);
        Assert.False(deletedTour!.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        var result = await _repository.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingTour_ReturnsTrue()
    {
        var tour = new Tour { TourName = "Exists Tour", Place = "Place", Days = 5, Price = 100m, Locations = "Locations", TourInfo = "Info", IsActive = true };
        await _context.Tours.AddAsync(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(tour.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingTour_ReturnsFalse()
    {
        var result = await _repository.ExistsAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithEmptyTerm_ReturnsAllTours()
    {
        var tours = new List<Tour>
        {
            new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true },
            new Tour { TourName = "Tour2", Place = "Place2", Days = 7, Price = 200m, Locations = "Loc2", TourInfo = "Info2", IsActive = true }
        };
        await _context.Tours.AddRangeAsync(tours);
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ReturnsMatchingTours()
    {
        var tours = new List<Tour>
        {
            new Tour { TourName = "Paris Tour", Place = "Paris", Days = 5, Price = 100m, Locations = "Loc1", TourInfo = "Info1", IsActive = true },
            new Tour { TourName = "London Tour", Place = "London", Days = 7, Price = 200m, Locations = "Loc2", TourInfo = "Info2", IsActive = true }
        };
        await _context.Tours.AddRangeAsync(tours);
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("Paris");

        Assert.Single(result);
        Assert.Equal("Paris Tour", result.First().TourName);
    }

    [Fact]
    public async Task SearchAsync_WithWhitespaceTerm_ReturnsAllTours()
    {
        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 100m, Locations = "Locations", TourInfo = "Info", IsActive = true };
        await _context.Tours.AddAsync(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("   ");

        Assert.Single(result);
    }
}
