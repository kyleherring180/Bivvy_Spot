namespace BivvySpot.Contracts.v1.Request;

public record GetGpxUploadSasRequest(string Extension, string ContentType, long ContentLength);