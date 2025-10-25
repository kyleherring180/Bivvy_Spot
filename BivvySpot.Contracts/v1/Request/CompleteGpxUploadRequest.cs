namespace BivvySpot.Contracts.v1.Request;

public record CompleteGpxUploadRequest(string BlobKey, string OriginalFileName, string ContentType, string? ChecksumSha256);