namespace BivvySpot.Contracts.v1.Request;

public record GetPhotoUploadSasItem(string Extension, string ContentType, long ContentLength);