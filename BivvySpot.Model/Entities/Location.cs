using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Entities;
using NetTopologySuite.Geometries;

public class Location : BaseEntity
{
    private readonly List<Location> _children = new();
    private readonly List<PostLocation> _postLocations = new();
    private readonly List<LocationAltName> _altNames = new();
    
    public Guid Id { get; init; }
    public string Name { get; private set; } = null!;
    public LocationType LocationType { get; private set; }
    public string? CountryCode { get; private set; } // ISO-3166-1 alpha-2
    public Point? Point { get; private set; } // SRID 4326
    public Polygon? Boundary { get; private set; } // SRID 4326
    public double? Elevation { get; private set; }
    public Guid? ParentId { get; private set; }
    public DateTimeOffset UpdatedDate { get; private set; }
    public DateTimeOffset? DeletedDate { get; private set; }

    public Location? Parent { get; set; }
    public IReadOnlyCollection<Location> Children => _children.AsReadOnly();
    public IReadOnlyCollection<PostLocation> PostLocations => _postLocations.AsReadOnly();
    public IReadOnlyCollection<LocationAltName> AltNames => _altNames.AsReadOnly();
    
    private Location() {/* private constructor for EF */}
    
    public Location(string name, LocationType type, Point? point, Polygon? boundary, string? countryCode, Guid? parentId, double? elevation = null)
    {
        Id = Guid.NewGuid();
        Rename(name);
        LocationType = type;
        SetPoint(point);
        SetBoundary(boundary);
        SetCountry(countryCode);
        ParentId = parentId;
        if (elevation is >= 0) Elevation = elevation;
        SetCreatedDate(); UpdatedDate = CreatedDate;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        Name = name.Trim();
        Touch();
    }

    public void SetCountry(string? cc)
    {
        CountryCode = string.IsNullOrWhiteSpace(cc) ? null : cc.Trim().ToUpperInvariant();
        Touch();
    }

    public void SetPoint(Point? p)
    {
        if (p is not null) p.SRID = 4326;
        Point = p; Touch();
    }

    public void SetBoundary(Polygon? poly)
    {
        if (poly is not null) poly.SRID = 4326;
        Boundary = poly; Touch();
    }
    
    // Add single alt name (idempotent by case-insensitive compare)
    public void AddAltName(string name, string? language = null)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        var trimmed = name.Trim();
        if (_altNames.Any(a => string.Equals(a.Name, trimmed, StringComparison.OrdinalIgnoreCase)))
            return;

        _altNames.Add(new LocationAltName(Id, trimmed, language));
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    // Replace all alt names with a new set
    public void SetAltNames(IEnumerable<(string name, string? lang)> items)
    {
        _altNames.Clear();
        foreach (var (name, lang) in items)
            AddAltName(name, lang);
    }

    public void SetParent(Guid? parentId) { ParentId = parentId; Touch(); }
    public void SoftDelete() { DeletedDate = DateTimeOffset.UtcNow; Touch(); }

    private void Touch() => UpdatedDate = DateTimeOffset.UtcNow;
}