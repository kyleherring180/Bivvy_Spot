using BivvySpot.Contracts.Shared;
using BivvySpot.Model.Dtos;

namespace BivvySpot.Contracts.v1.Request;

public record CreateLocationRequest(
    string Name,
    LocationType Type,
    double? Latitude,
    double? Longitude,
    string? BoundaryGeoJson,
    string? CountryCode,
    Guid? ParentId,
    double? Elevation,
    IReadOnlyCollection<AltNameDto>? AltNames
    );