using BivvySpot.Model.Dtos;

namespace BivvySpot.Presentation.v1.MapToModel;

public static class LocationExtensions
{
    public static CreateLocationDto ToDto(this Contracts.v1.Request.CreateLocationRequest req)
    {
        return new CreateLocationDto
        {
            Name = req.Name,
            Type = req.Type.ToModel(),
            Latitude = req.Latitude,
            Longitude = req.Longitude,
            BoundaryGeoJson = req.BoundaryGeoJson,
            CountryCode = req.CountryCode,
            ParentId = req.ParentId,
            Elevation = req.Elevation,
            AltNames = req.AltNames
        };
    }
    
    public static CreateLocationSuggestionDto ToDto(this Contracts.v1.Request.CreateLocationSuggestionRequest req)
    {
        return new CreateLocationSuggestionDto
        {
            Name = req.Name,
            LocationType = req.Type.ToModel(),
            Latitude = req.Latitude,
            Longitude = req.Longitude,
            CountryCode = req.CountryCode,
            ParentId = req.ParentId,
            Note = req.Note
        };
    }
    
    public static Model.Enums.LocationType ToModel(this Contracts.Shared.LocationType type) => type switch
    {
        Contracts.Shared.LocationType.Country => Model.Enums.LocationType.Country,
        Contracts.Shared.LocationType.Region => Model.Enums.LocationType.Region,
        Contracts.Shared.LocationType.Peak => Model.Enums.LocationType.Peak,
        Contracts.Shared.LocationType.Hut => Model.Enums.LocationType.Hut,
        Contracts.Shared.LocationType.Trailhead => Model.Enums.LocationType.Trailhead,
        Contracts.Shared.LocationType.Crag => Model.Enums.LocationType.Crag,
        Contracts.Shared.LocationType.Range => Model.Enums.LocationType.Range,
        _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected location type value: {type}"),
    };
}