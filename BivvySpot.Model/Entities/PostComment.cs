namespace BivvySpot.Model.Entities;

public class PostComment : BaseEntity
{
    private readonly List<PostComment> _replies = new();
    
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Body { get; set; } = null!;
    public Guid? ParentCommentId { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }

    public Post Post { get; set; } = null!;
    public User User { get; set; } = null!;
    public PostComment? Parent { get; set; }
    public IReadOnlyCollection<PostComment> Replies => _replies.AsReadOnly();
    
    private PostComment() { /* private constructor for EF */}
}