using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Entities;
using NetTopologySuite.Geometries;

public class Location
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public LocationType LocationType { get; set; }
    public string? CountryCode { get; set; } // ISO-3166-1 alpha-2
    public Point? Point { get; set; } // SRID 4326
    public Polygon? Boundary { get; set; } // SRID 4326
    public double? Elevation { get; set; }
    public Guid? ParentId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Location? Parent { get; set; }
    public ICollection<Location> Children { get; set; } = new List<Location>();
    public ICollection<PostLocation> PostLocations { get; set; } = new List<PostLocation>();
    public ICollection<LocationAltName> AltNames { get; set; } = new List<LocationAltName>();
}