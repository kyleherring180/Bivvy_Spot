using BivvySpot.Application.Abstractions.Security;
using BivvySpot.Presentation.Security;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace BivvySpot.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers MVC controllers contained in the Presentation assembly.
    /// Call this once from Program.cs in your host.
    /// </summary>
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        var assembly = typeof(PresentationAssemblyMarker).Assembly;

        // Create the MVC builder and add this assembly as an application part
        var mvc = services.AddControllers();
        mvc.PartManager.ApplicationParts.Add(new AssemblyPart(assembly));

        // Optional but handy: register controllers for DI (constructor-injected services)
        mvc.AddControllersAsServices();

        // (Optional) common MVC tweaks for your presentation layer:
        // services.Configure<ApiBehaviorOptions>(o => o.SuppressInferBindingSourcesForParameters = true);

        return services;
    }

    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        return services
            .AddHttpContextAccessor()
            .AddScoped<IAuthContextProvider, AuthContextProvider>();
    }

    // Empty marker type to get this assembly without referencing a specific controller type
    internal sealed class PresentationAssemblyMarker { }
}