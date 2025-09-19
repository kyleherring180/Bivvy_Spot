namespace BivvySpot.Model.Entities;

public class LocationAltName
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string Name { get; set; } = null!;
    public string? Language { get; set; }

    public Location Location { get; set; } = null!;
}