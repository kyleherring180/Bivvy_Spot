using BivvySpot.Model.Enums;
using NetTopologySuite.Geometries;

namespace BivvySpot.Model.Dtos;

public class CreateLocationSuggestionDto
{
    public string Name { get; set; } = null!;
    public LocationType LocationType { get; set; }
    public string? CountryCode { get; set; }
    public double Longitude { get; set; }   // require a point for suggestions
    public double Latitude { get; set; }
    public Guid? ParentId { get; set; }
    public string? Note { get; set; }
    public Guid SubmittedByUserId { get; set; }
    public SuggestionStatus Status { get; set; } = SuggestionStatus.Open;
    public Guid? ApprovedLocationId { get; set; }
}