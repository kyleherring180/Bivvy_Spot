namespace BivvySpot.Model.Entities;

public class PostPhoto : BaseEntity
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string StorageKey { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int SortOrder { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }

    public Post Post { get; set; } = null!;
}