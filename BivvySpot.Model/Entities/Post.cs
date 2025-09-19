using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Entities;

public class Post : BaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string? RouteName { get; set; }
    public string Body { get; set; } = null!;
    public string? Season { get; set; } // Consider a month bitmask later
    public int ElevationGain { get; set; } // meters
    public int Duration { get; set; } // minutes
    public int LikeCount { get; set; }
    public int SaveCount { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public PostStatus Status { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }

    public User User { get; set; } = null!;
    public ICollection<PostPhoto> Photos { get; set; } = new List<PostPhoto>();
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    public ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
    public ICollection<GpxTrack> GpxTracks { get; set; } = new List<GpxTrack>();
    public ICollection<PostLocation> PostLocations { get; set; } = new List<PostLocation>();
    public ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<PostDifficulty> PostDifficulties { get; set; } = new List<PostDifficulty>();
}