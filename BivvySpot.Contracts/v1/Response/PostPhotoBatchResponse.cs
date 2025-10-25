namespace BivvySpot.Contracts.v1.Response;

public record PostPhotoBatchResponse(IReadOnlyList<PostPhotoResponse> Items);