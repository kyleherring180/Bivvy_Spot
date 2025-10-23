using BivvySpot.Application.Abstractions.Infrastructure;
using BivvySpot.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BivvySpot.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Storage");
        var useManagedIdentityValue = section["UseManagedIdentity"];
        var useManagedIdentity = bool.TryParse(useManagedIdentityValue, out var parsed) && parsed;

        if (useManagedIdentity)
        {
            var accountName = section["AccountName"]!;
            var uri = new Uri($"https://{accountName}.blob.core.windows.net");
            var credential = new Azure.Identity.DefaultAzureCredential();
            services.AddSingleton(new Azure.Storage.Blobs.BlobServiceClient(uri, credential));
        }
        else
        {
            var conn = section["ConnectionString"]!;
            services.AddSingleton(new Azure.Storage.Blobs.BlobServiceClient(conn));
        }

        services.Configure<StorageOptions>(options => section.Bind(options));
        services.AddScoped<IObjectStorage, AzureBlobObjectStorage>();
        return services;
    }
}