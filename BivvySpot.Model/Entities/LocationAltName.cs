namespace BivvySpot.Model.Entities;

public class LocationAltName
{
    public Guid Id { get; init; }
    public Guid LocationId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Language { get; private set; }

    public Location Location { get; private set; } = null!;
    
    private LocationAltName() { /* private constructor for EF */}
    
    public LocationAltName(Guid locationId, string name, string? language)
    {
        Id = Guid.NewGuid();
        LocationId = locationId;
        Name = name.Trim();
        Language = string.IsNullOrWhiteSpace(language) ? null : language.Trim().ToLowerInvariant(); // e.g., "fr", "it"
    }
}