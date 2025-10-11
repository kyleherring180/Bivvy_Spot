namespace BivvySpot.Model.Entities;

public class PostTag
{
    public Guid Id { get; init; }
    public Guid TagId { get; private set; }
    public Guid PostId { get; private set; }
    
    public Tag Tag { get; private set; }
    public Post Post { get; private set; }
    
    private PostTag() { /* private constructor for EF */}
    
    // Maintain a clear parameter order to avoid confusion: (postId, tagId)
    public PostTag(Guid postId, Guid tagId)
    {
        Id = Guid.NewGuid();
        PostId = postId;
        TagId = tagId;
    }
}