using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Entities;

public class Report : BaseEntity
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid ReporterId { get; set; }
    public string Reason { get; set; } = null!;
    public Guid? ResolvedBy { get; set; }
    public DateTimeOffset? ResolvedDate { get; set; }
    public string? ModeratorNote { get; set; }
    public ReportStatus Status { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }

    public Post Post { get; set; } = null!;
    public User Reporter { get; set; } = null!;
    public User? Resolver { get; set; }
    
    private Report() { /* private constructor for EF */}
}