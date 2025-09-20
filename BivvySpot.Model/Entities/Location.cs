using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Entities;
using NetTopologySuite.Geometries;

public class Location : BaseEntity
{
    private readonly List<Location> _children = new();
    private readonly List<PostLocation> _postLocations = new();
    private readonly List<LocationAltName> _altNames = new();
    
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public LocationType LocationType { get; set; }
    public string? CountryCode { get; set; } // ISO-3166-1 alpha-2
    public Point? Point { get; set; } // SRID 4326
    public Polygon? Boundary { get; set; } // SRID 4326
    public double? Elevation { get; set; }
    public Guid? ParentId { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }

    public Location? Parent { get; set; }
    public IReadOnlyCollection<Location> Children => _children.AsReadOnly();
    public IReadOnlyCollection<PostLocation> PostLocations => _postLocations.AsReadOnly();
    public IReadOnlyCollection<LocationAltName> AltNames => _altNames.AsReadOnly();
    
    private Location() { /* private constructor for EF */}
}