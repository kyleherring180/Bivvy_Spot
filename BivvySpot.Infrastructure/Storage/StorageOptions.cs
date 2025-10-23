namespace BivvySpot.Infrastructure.Storage;

public sealed class StorageOptions
{
    public bool UseManagedIdentity { get; init; }
    public string? ConnectionString { get; init; }
    public string? AccountName { get; init; }
    public ContainersOptions Containers { get; init; } = new();
    public string? CdnBaseUrl { get; init; } // optional for read URLs
    public sealed class ContainersOptions
    {
        public string Photos { get; init; } = "photos";
        public string Gpx { get; init; } = "gpx";
    }
}