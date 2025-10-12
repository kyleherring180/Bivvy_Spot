using BivvySpot.Contracts.Shared;

namespace BivvySpot.Contracts.v1.Response;

public record LocationResponse(
    Guid Id,
    string Name,
    LocationType Type,
    string? CountryCode,
    Guid? ParentId,
    double? Elevation
    );