namespace BivvySpot.Contracts.v1.Response;

public record GetGpxUploadSasResponse(Uri UploadUrl, string BlobKey);