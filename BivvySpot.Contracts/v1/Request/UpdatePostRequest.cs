using BivvySpot.Contracts.Shared;

namespace BivvySpot.Contracts.v1.Request;

public record UpdatePostRequest(
    string? Title,
    string? Body,
    Season? Season,
    int? ElevationGain,
    int? Duration,
    string? RouteName,
    PostStatus? Status,
    byte[]? RowVersion,
    IReadOnlyCollection<string>? Tags,
    IReadOnlyCollection<Guid>? LocationIds
    );