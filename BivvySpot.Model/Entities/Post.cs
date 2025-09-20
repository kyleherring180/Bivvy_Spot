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
}