namespace BivvySpot.Contracts.v1.Request;

public record CompletePhotoUploadBatchRequest(IReadOnlyList<CompletePhotoUploadItem> Items);