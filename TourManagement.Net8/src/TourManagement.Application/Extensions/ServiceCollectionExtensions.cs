using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Extensions;

/// <summary>
/// Extension methods for registering application services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IUserInfoService, UserInfoService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}
