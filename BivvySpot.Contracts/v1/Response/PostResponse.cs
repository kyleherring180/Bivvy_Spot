using BivvySpot.Contracts.Shared;

namespace BivvySpot.Contracts.v1.Response;

public record PostResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    string Title,
    string? RouteName,
    string Body,
    Season Season,
    int ElevationGain,
    int Duration,
    PostStatus Status,
    DateTimeOffset UpdatedDate,
    IReadOnlyCollection<string> Tags
    );