using BivvySpot.Contracts.Shared;

namespace BivvySpot.Contracts.v1.Request;

public record CreateLocationSuggestionRequest(
    string Name,
    LocationType Type,
    double Latitude,
    double Longitude,
    string? CountryCode,
    Guid? ParentId,
    string? Note
    );