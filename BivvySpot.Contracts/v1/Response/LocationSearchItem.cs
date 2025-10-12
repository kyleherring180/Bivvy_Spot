using BivvySpot.Contracts.Shared;

namespace BivvySpot.Contracts.v1.Response;

public record LocationSearchItem(
    Guid Id,
    string Name,
    LocationType Type,
    string? MatchAlias,     // null if primary name matched
    double? Lat,
    double? Lon
    );