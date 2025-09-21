using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BivvySpot.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddApplicationWithoutConfiguration();
    }
    
    private static IServiceCollection AddApplicationWithoutConfiguration(this IServiceCollection services)
    {
        return services
            .AddScoped<IAccountService, AccountService>();
    }
}