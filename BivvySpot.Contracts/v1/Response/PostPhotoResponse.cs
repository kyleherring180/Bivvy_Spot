namespace BivvySpot.Contracts.v1.Response;

public record PostPhotoResponse(Guid Id, string StorageKey, string Url, int Order, int? Width, int? Height);