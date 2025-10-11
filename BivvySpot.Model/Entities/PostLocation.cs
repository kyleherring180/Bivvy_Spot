namespace BivvySpot.Model.Entities;

public class PostLocation
{
    public Guid Id { get; init; }
    public Guid PostId { get; private set; }
    public Guid LocationId { get; private set; }
    public int Order { get; private set; } // 0 = primary

    public Post Post { get; private set; } = null!;
    public Location Location { get; private set; } = null!;
    
    private PostLocation() { /* private constructor for EF */}

    public PostLocation(Guid postId, Guid locationId, int order)
    {
        Id = Guid.NewGuid();
        PostId = postId;
        LocationId = locationId;
        Order = Math.Max(0, order);
    }
}