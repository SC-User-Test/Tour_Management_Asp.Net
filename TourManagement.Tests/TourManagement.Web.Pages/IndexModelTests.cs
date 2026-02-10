using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages;

namespace TourManagement.Web.Pages.Tests;

public class IndexModelTests
{
    private readonly Mock<ILogger<IndexModel>> _mockLogger;
    private readonly IndexModel _model;

    public IndexModelTests()
    {
        _mockLogger = new Mock<ILogger<IndexModel>>();
        _model = new IndexModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_InitializesWithLogger()
    {
        // Arrange & Act
        var model = new IndexModel(_mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_ExecutesSuccessfully()
    {
        // Arrange
        var model = new IndexModel(_mockLogger.Object);

        // Act
        model.OnGet();

        // Assert - No exception thrown
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_LogsInformation()
    {
        // Arrange
        var model = new IndexModel(_mockLogger.Object);

        // Act
        model.OnGet();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Home page accessed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void OnGet_CanBeCalledMultipleTimes()
    {
        // Arrange
        var model = new IndexModel(_mockLogger.Object);

        // Act
        model.OnGet();
        model.OnGet();
        model.OnGet();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(3));
    }
}
