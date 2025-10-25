namespace BivvySpot.Contracts.v1.Request;

public record CompletePhotoUploadItem(string BlobKey, string OriginalFileName, string ContentType, string? ChecksumSha256);