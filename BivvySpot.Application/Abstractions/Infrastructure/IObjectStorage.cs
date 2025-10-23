namespace BivvySpot.Application.Abstractions.Infrastructure;

public interface IObjectStorage
{
    Task<string> UploadAsync(string container, string key, Stream content, string contentType, string? cacheControl, CancellationToken ct);
    Task DeleteAsync(string container, string key, CancellationToken ct);
    Uri GetReadUri(string container, string key); // optionally CDN
    Task<Uri> GetSasForClientUploadAsync(string container, string key, TimeSpan ttl, CancellationToken ct);
}