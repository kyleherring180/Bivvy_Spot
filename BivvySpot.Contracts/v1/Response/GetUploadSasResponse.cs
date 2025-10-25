namespace BivvySpot.Contracts.v1.Response;

public record GetUploadSasResponse(Uri UploadUrl, string BlobKey);