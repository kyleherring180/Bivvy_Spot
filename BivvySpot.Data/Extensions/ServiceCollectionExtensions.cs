using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BivvySpot.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BivvySpotContext>(o => o.UseSqlServer(connectionString));
        services.AddDataWithoutContext();
        return services;
    }
    
    /// <summary>
    /// Configures Dependency Injection for Repository classes without adding DbContext.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDataWithoutContext(this IServiceCollection services)
    {
        return services
            .AddScoped<IPostRepository, PostRepository>()
            .AddScoped<IUserRepository, UserRepository>();
    }
}