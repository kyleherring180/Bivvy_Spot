namespace BivvySpot.Contracts.v1.Request;

public record GetPhotoUploadSasBatchRequest(IReadOnlyList<GetPhotoUploadSasItem> Items);