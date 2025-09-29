using BivvySpot.Contracts.Shared;

namespace BivvySpot.Contracts.v1.Request;

public record CreatePostRequest(
    string Title, 
    string Body, 
    Season Season, 
    int ElevationGain, 
    int Duration, 
    string? RouteName,
    IReadOnlyCollection<string>? Tags
    );