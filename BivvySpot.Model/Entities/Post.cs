using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Entities;

public class Post : BaseEntity
{
    private readonly List<PostPhoto> _photos = new();
    private readonly List<PostTag> _postTags = new();
    private readonly List<Interaction> _interactions = new();
    private readonly List<GpxTrack> _gpxTracks = new();
    private readonly List<PostLocation> _postLocations = new();
    private readonly List<PostComment> _comments = new();
    private readonly List<Report> _reports = new();
    private readonly List<PostDifficulty> _postDifficulties = new();
    
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string? RouteName { get; set; }
    public string Body { get; set; } = null!;
    public Season Season { get; set; } // Consider a month bitmask later
    public int ElevationGain { get; set; } // meters
    public int Duration { get; set; } // minutes
    public int LikeCount { get; set; }
    public int SaveCount { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public PostStatus Status { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }

    public User User { get; set; } = null!;
    public IReadOnlyCollection<PostPhoto> Photos => _photos.AsReadOnly();
    public IReadOnlyCollection<PostTag> PostTags => _postTags.AsReadOnly();
    public IReadOnlyCollection<Interaction> Interactions => _interactions.AsReadOnly();
    public IReadOnlyCollection<GpxTrack> GpxTracks => _gpxTracks.AsReadOnly();
    public IReadOnlyCollection<PostLocation> PostLocations => _postLocations.AsReadOnly();
    public IReadOnlyCollection<PostComment> Comments => _comments.AsReadOnly();
    public IReadOnlyCollection<Report> Reports => _reports.AsReadOnly();
    public IReadOnlyCollection<PostDifficulty> PostDifficulties => _postDifficulties.AsReadOnly();
    
    private Post() { /* private constructor for EF */}
    
    public Post(
        Guid userId,
        string title,
        string body,
        Season season,
        int elevationGain,
        int duration,
        string? routeName = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Title = title.Trim();
        Body = body; // allow markdown
        Season = season;
        ElevationGain = Math.Max(0, elevationGain);
        Duration = Math.Max(0, duration);
        RouteName = string.IsNullOrWhiteSpace(routeName) ? null : routeName.Trim();
        LikeCount = 0;
        SaveCount = 0;
        Status = PostStatus.Draft;

        SetCreatedDate();
        UpdatedDate = CreatedDate;
    }

    public void Update(
        string? title = null,
        string? routeName = null,
        string? body = null,
        Season? season = null,
        int? elevationGain = null,
        int? duration = null,
        PostStatus? status = null)
    {
        if (!string.IsNullOrWhiteSpace(title)) Title = title.Trim();
        if (routeName is not null) RouteName = string.IsNullOrWhiteSpace(routeName) ? null : routeName.Trim();
        if (body is not null) Body = body;
        if (season.HasValue) Season = season.Value;
        if (elevationGain.HasValue) ElevationGain = Math.Max(0, elevationGain.Value);
        if (duration.HasValue) Duration = Math.Max(0, duration.Value);
        if (status.HasValue) Status = status.Value;
        UpdatedDate = DateTimeOffset.UtcNow;
    }
}