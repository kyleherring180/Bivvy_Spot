namespace BivvySpot.Model.Entities;

public class PostLocation
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid LocationId { get; set; }
    public int Order { get; set; } // 0 = primary

    public Post Post { get; set; } = null!;
    public Location Location { get; set; } = null!;
    
    private PostLocation() { /* private constructor for EF */}
}