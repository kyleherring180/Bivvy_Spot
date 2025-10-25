namespace BivvySpot.Contracts.v1.Response;

public record GpxTrackResponse(Guid Id, string StorageKey, string? PreviewGeoJsonKey, long SizeBytes, double? Distance, double? AscentM);