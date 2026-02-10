using Xunit;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Extensions;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Application.Services;
using AutoMapper;

namespace TourManagement.UnitTests.TourManagement.Application.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_RegistersAutoMapper()
    {
        var services = new ServiceCollection();

        services.AddApplicationServices();

        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();

        Assert.NotNull(mapper);
    }

    [Fact]
    public void AddApplicationServices_RegistersTourService()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddApplicationServices();

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITourService));
        Assert.NotNull(descriptor);
        Assert.Equal(typeof(TourService), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddApplicationServices_RegistersUserInfoService()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddApplicationServices();

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IUserInfoService));
        Assert.NotNull(descriptor);
        Assert.Equal(typeof(UserInfoService), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddApplicationServices_RegistersBookingService()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddApplicationServices();

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IBookingService));
        Assert.NotNull(descriptor);
        Assert.Equal(typeof(BookingService), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddApplicationServices_RegistersScopedServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddApplicationServices();

        var serviceProvider = services.BuildServiceProvider();
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();

        var service1 = scope1.ServiceProvider.GetService<ITourService>();
        var service2 = scope2.ServiceProvider.GetService<ITourService>();

        Assert.NotNull(service1);
        Assert.NotNull(service2);
    }

    [Fact]
    public void AddApplicationServices_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddApplicationServices();

        Assert.Same(services, result);
    }
}
