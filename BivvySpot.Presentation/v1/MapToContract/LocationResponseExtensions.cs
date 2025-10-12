namespace BivvySpot.Presentation.v1.MapToContract;

public static class LocationResponseExtensions
{
    public static Contracts.v1.Response.LocationResponse ToResponse(this Model.Entities.Location model)
    {
        return new Contracts.v1.Response.LocationResponse(
            model.Id,
            model.Name,
            model.LocationType.ToContract(),
            model.CountryCode,
            model.ParentId,
            model.Elevation
        );
    }
    
    public static Contracts.Shared.LocationType ToContract(this Model.Enums.LocationType type)
        => type switch
        {
            Model.Enums.LocationType.Country => Contracts.Shared.LocationType.Country,
            Model.Enums.LocationType.Region => Contracts.Shared.LocationType.Region,
            Model.Enums.LocationType.Peak => Contracts.Shared.LocationType.Peak,
            Model.Enums.LocationType.Hut => Contracts.Shared.LocationType.Hut,
            Model.Enums.LocationType.Trailhead => Contracts.Shared.LocationType.Trailhead,
            Model.Enums.LocationType.Crag => Contracts.Shared.LocationType.Crag,
            Model.Enums.LocationType.Range => Contracts.Shared.LocationType.Range,
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected location type value: {type}"),
        };
}