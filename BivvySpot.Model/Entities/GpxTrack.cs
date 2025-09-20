using BivvySpot.Model.Enums;
using NetTopologySuite.Geometries;

namespace BivvySpot.Model.Entities;

public class GpxTrack : BaseEntity
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string StorageKey { get; set; } = null!;
    public GpxStatus Status { get; set; }
    public double Distance { get; set; } // km
    public long SizeBytes { get; set; }
    public string ChecksumSha256 { get; set; } = null!;
    public bool HasTimestamps { get; set; }
    public int AscentM { get; set; }
    public Polygon? Bbox { get; set; } // SRID 4326
    public string? PreviewGeoJsonKey { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }

    public Post Post { get; set; } = null!;
    
    private GpxTrack() { /* private constructor for EF */}
}