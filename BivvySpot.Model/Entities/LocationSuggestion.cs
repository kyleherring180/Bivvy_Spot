using BivvySpot.Model.Enums;
using NetTopologySuite.Geometries;

namespace BivvySpot.Model.Entities;

public class LocationSuggestion : BaseEntity
{
    public Guid Id { get; init; }
    public string Name { get; private set; } = null!;
    public LocationType LocationType { get; private set; }
    public string? CountryCode { get; private set; }
    public Point Point { get; private set; }   // require a point for suggestions
    public Guid? ParentId { get; private set; }
    public string? Note { get; private set; }
    public Guid SubmittedByUserId { get; private set; }
    public SuggestionStatus Status { get; private set; } = SuggestionStatus.Open;
    public Guid? ApprovedLocationId { get; private set; }
    public DateTimeOffset UpdatedDate { get; private set; }

    private LocationSuggestion() {/* private constructor for EF */}
    
    public LocationSuggestion(Guid submittedByUserId, string name, LocationType type, Point point, string? countryCode, Guid? parentId, string? note)
    {
        Id = Guid.NewGuid();
        SubmittedByUserId = submittedByUserId;
        Name = name.Trim();
        LocationType = type;
        Point = point; Point.SRID = 4326;
        CountryCode = string.IsNullOrWhiteSpace(countryCode) ? null : countryCode.Trim().ToUpperInvariant();
        ParentId = parentId;
        Note = note;
        SetCreatedDate(); 
        UpdatedDate = CreatedDate;
    }

    public void MarkApproved(Guid locationId) { Status = SuggestionStatus.Approved; ApprovedLocationId = locationId; UpdatedDate = DateTimeOffset.UtcNow; }
    public void MarkRejected(string? reason = null) { Status = SuggestionStatus.Rejected; UpdatedDate = DateTimeOffset.UtcNow; }
}