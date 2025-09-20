namespace BivvySpot.Model.Entities;

public class PostTag
{
    public Guid Id { get; set; }
    public Guid TagId { get; set; }
    public Guid PostId { get; set; }
    
    public Tag Tag { get; set; }
    public Post Post { get; set; }
    
    private PostTag() { /* private constructor for EF */}
}