using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Dtos;

public class CreateLocationDto
{
    public string Name;
    public LocationType Type;
    public double? Latitude;
    public double? Longitude;
    public string? BoundaryGeoJson;
    public string? CountryCode;
    public Guid? ParentId;
    public double? Elevation;
    public IReadOnlyCollection<AltNameDto>? AltNames;
}