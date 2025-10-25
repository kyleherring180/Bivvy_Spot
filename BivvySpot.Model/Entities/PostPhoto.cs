namespace BivvySpot.Model.Entities;

public class PostPhoto : BaseEntity
{
    public Guid Id { get; init; }
    public Guid PostId { get; private set; }
    public string StorageKey { get; private set; } = null!;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string ContentType { get; private set; } = null!;
    public string ChecksumSha256 { get; private set; } = string.Empty;
    public string? AltText { get; private set; }
    public string? Caption { get; private set; }
    public int SortOrder { get; private set; }
    public DateTimeOffset UpdatedDate { get; private set; }
    public DateTimeOffset? DeletedDate { get; private set; }

    public Post Post { get; private set; } = null!;
    
    private PostPhoto() { /* private constructor for EF */}

    public PostPhoto(Guid postId, string storageKey, string contentType, string checksumSha256, int order)
    {
        if (postId == Guid.Empty) throw new ArgumentException("postId required.");
        if (string.IsNullOrWhiteSpace(storageKey)) throw new ArgumentException("storageKey required.");
        if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentException("contentType required.");

        Id = Guid.NewGuid();
        PostId = postId;
        StorageKey = storageKey.Trim();
        ContentType = contentType.Trim().ToLowerInvariant();
        ChecksumSha256 = checksumSha256?.Trim().ToLowerInvariant() ?? "";
        SortOrder = order;
        SetCreatedDate();
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void SetOrder(int order) { SortOrder = order; UpdatedDate = DateTimeOffset.UtcNow; }
    public void SetDimensions(int width, int height) { Width = width; Height = height; UpdatedDate = DateTimeOffset.UtcNow; }
    public void SoftDelete() { DeletedDate = DateTimeOffset.UtcNow; UpdatedDate = DateTimeOffset.UtcNow; }
}