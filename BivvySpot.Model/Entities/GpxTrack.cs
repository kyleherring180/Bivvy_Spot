using BivvySpot.Model.Enums;
using NetTopologySuite.Geometries;

namespace BivvySpot.Model.Entities;

public class GpxTrack : BaseEntity
{
    public Guid Id { get; init; }
    public Guid PostId { get; private set; }
    public string StorageKey { get; private set; } = null!;
    public GpxStatus Status { get; private set; }
    public double Distance { get; private set; } // km
    public long SizeBytes { get; private set; }
    public string ChecksumSha256 { get; private set; } = null!;
    public bool HasTimestamps { get; private set; }
    public int AscentM { get; private set; }
    public Polygon? Bbox { get; private set; } // SRID 4326
    public string? PreviewGeoJsonKey { get; private set; }
    public DateTimeOffset UpdatedDate { get; private set; }
    public DateTimeOffset? DeletedDate { get; private set; }

    public Post Post { get; set; } = null!;
    
    private GpxTrack() { /* private constructor for EF */}

    public GpxTrack(Guid postId, string storageKey, string checksumSha256, long sizeBytes)
    {
        if (postId == Guid.Empty) throw new ArgumentException("postId required.");
        if (string.IsNullOrWhiteSpace(storageKey)) throw new ArgumentException("storageKey required.");
        if (sizeBytes < 0) throw new ArgumentException("sizeBytes invalid.");

        Id = Guid.NewGuid();
        PostId = postId;
        StorageKey = storageKey.Trim();
        ChecksumSha256 = checksumSha256?.Trim().ToLowerInvariant() ?? "";
        SizeBytes = sizeBytes;
        SetCreatedDate();
        UpdatedDate = DateTimeOffset.UtcNow;
    }
    
    public void SetDerived(double distance, int ascent, string? previewKey)
    {
        Distance = distance; AscentM = ascent; PreviewGeoJsonKey = previewKey;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void SoftDelete() { DeletedDate = DateTimeOffset.UtcNow; UpdatedDate = DateTimeOffset.UtcNow; }
}