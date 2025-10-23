using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using BivvySpot.Application.Abstractions.Infrastructure;
using Microsoft.Extensions.Options;

namespace BivvySpot.Infrastructure.Storage;

public sealed class AzureBlobObjectStorage(BlobServiceClient svc, IOptions<StorageOptions> opts) : IObjectStorage
{
    private readonly StorageOptions _opts = opts.Value;

    private BlobClient Client(string container, string key)
    {
        var c = svc.GetBlobContainerClient(container);
        c.CreateIfNotExists(PublicAccessType.None);
        return c.GetBlobClient(key);
    }

    public async Task<string> UploadAsync(string container, string key, Stream content, string contentType, string? cacheControl, CancellationToken ct)
    {
        var blob = Client(container, key);
        var headers = new BlobHttpHeaders { ContentType = contentType };
        if (!string.IsNullOrWhiteSpace(cacheControl)) headers.CacheControl = cacheControl;
        await blob.UploadAsync(content, new BlobUploadOptions { HttpHeaders = headers }, ct);
        return key;
    }

    public Task DeleteAsync(string container, string key, CancellationToken ct)
        => Client(container, key).DeleteIfExistsAsync(cancellationToken: ct);

    public Uri GetReadUri(string container, string key)
    {
        if (!string.IsNullOrWhiteSpace(_opts.CdnBaseUrl))
            return new Uri($"{_opts.CdnBaseUrl.TrimEnd('/')}/{container}/{key}");
        return svc.GetBlobContainerClient(container).GetBlobClient(key).Uri;
    }

    public async Task<Uri> GetSasForClientUploadAsync(string container, string key, TimeSpan ttl, CancellationToken ct)
    {
        var blob = Client(container, key);
        // Ensure container exists
        await blob.GetParentBlobContainerClient().CreateIfNotExistsAsync(cancellationToken: ct);

        // Create a SAS giving the client permission to create/write this one blob
        BlobSasBuilder sas = new()
        {
            BlobContainerName = blob.BlobContainerName,
            BlobName = blob.Name,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
            ExpiresOn = DateTimeOffset.UtcNow.Add(ttl)
        };
        sas.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write | BlobSasPermissions.Add);

        // If using connection string/key: sign with account key
        if (!_opts.UseManagedIdentity)
        {
            var acct = svc.GetAccountInfo().Value; // forces a call; optional
        }

        // Easiest: fetch the account key from the service client’s connection (when not MI)
        // If you need a key-free approach, generate user delegation SAS with AAD creds (requires Azure AD + proper roles).
        var uri = blob.GenerateSasUri(sas);
        return uri;
    }
}