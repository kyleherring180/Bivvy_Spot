namespace BivvySpot.Model.Entities;

public class PostTag
{
    public Guid Id { get; init; }
    public Guid TagId { get; private set; }
    public Guid PostId { get; private set; }
    
    public Tag Tag { get; private set; }
    public Post Post { get; private set; }
    
    private PostTag() { /* private constructor for EF */}
    
    public PostTag(Guid tagId, Guid postId)
    {
        Id = Guid.NewGuid();
        TagId = tagId;
        PostId = postId;
    }
}