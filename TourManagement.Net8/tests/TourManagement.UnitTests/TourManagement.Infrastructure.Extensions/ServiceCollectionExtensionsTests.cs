using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TourManagement.Infrastructure.Extensions;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Repositories;
using System.Collections.Generic;

namespace TourManagement.UnitTests.TourManagement.Infrastructure.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructureServices_RegistersDbContext()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        services.AddInfrastructureServices(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<TourManagementDbContext>();

        Assert.NotNull(dbContext);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersTourRepository()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        services.AddLogging();

        services.AddInfrastructureServices(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<ITourRepository>();

        Assert.NotNull(repository);
        Assert.IsType<TourRepository>(repository);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersUserInfoRepository()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        services.AddLogging();

        services.AddInfrastructureServices(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<IUserInfoRepository>();

        Assert.NotNull(repository);
        Assert.IsType<UserInfoRepository>(repository);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersBookingRepository()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        services.AddLogging();

        services.AddInfrastructureServices(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<IBookingRepository>();

        Assert.NotNull(repository);
        Assert.IsType<BookingRepository>(repository);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersScopedRepositories()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        services.AddLogging();

        services.AddInfrastructureServices(configuration);

        var serviceProvider = services.BuildServiceProvider();
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();

        var repo1 = scope1.ServiceProvider.GetService<ITourRepository>();
        var repo2 = scope2.ServiceProvider.GetService<ITourRepository>();

        Assert.NotNull(repo1);
        Assert.NotNull(repo2);
        Assert.NotSame(repo1, repo2);
    }

    [Fact]
    public void AddInfrastructureServices_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        var result = services.AddInfrastructureServices(configuration);

        Assert.Same(services, result);
    }

    private IConfiguration CreateConfiguration()
    {
        var configData = new Dictionary<string, string>
        {
            {"ConnectionStrings:DefaultConnection", "Server=(localdb)\\mssqllocaldb;Database=TourManagementTest;Trusted_Connection=True;MultipleActiveResultSets=true"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
    }
}
