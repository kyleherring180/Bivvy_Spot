using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Entities;

public class Report : BaseEntity
{
    public Guid Id { get; init; }
    public Guid PostId { get; init; }
    public Guid ReporterId { get; init; }
    public string Reason { get; private set; } = null!;
    public Guid? ResolvedBy { get; private set; }
    public DateTimeOffset? ResolvedDate { get; private set; }
    public string? ModeratorNote { get; private set; }
    public ReportStatus Status { get; private set; }
    public DateTimeOffset UpdatedDate { get; private set; }
    public DateTimeOffset? DeletedDate { get; private set; }

    public Post Post { get; private set; } = null!;
    public User Reporter { get; private set; } = null!;
    public User? Resolver { get; private set; }
    
    private Report() { /* private constructor for EF */}

    public Report(Guid postId, Guid reporterId, string reason)
    {
        Id = Guid.NewGuid();
        PostId = postId;
        ReporterId = reporterId;
        Reason = reason;
        Status = ReportStatus.Open;
        UpdatedDate = DateTimeOffset.UtcNow;
        SetCreatedDate();
    }

    public void Moderate(ReportStatus status, Guid moderatorId, string? moderatorNote)
    {
        Status = status;
        ModeratorNote = moderatorNote;
        if (status == ReportStatus.Open)
        {
            ResolvedBy = null;
            ResolvedDate = null;
        }
        else
        {
            ResolvedBy = moderatorId;
            ResolvedDate = DateTimeOffset.UtcNow;
        }
        UpdatedDate = DateTimeOffset.UtcNow;
    }
}