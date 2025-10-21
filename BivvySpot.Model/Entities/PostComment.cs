namespace BivvySpot.Model.Entities;

public class PostComment : BaseEntity
{
    private readonly List<PostComment> _replies = new();
    
    public Guid Id { get; init; }
    public Guid PostId { get; init; }
    public Guid UserId { get; init; }
    public string Body { get; private set; } = null!;
    public Guid? ParentCommentId { get; private set; }
    public DateTimeOffset UpdatedDate { get; private set; }
    public DateTimeOffset? DeletedDate { get; private set; }

    public Post Post { get; private set; } = null!;
    public User User { get; private set; } = null!;
    public PostComment? Parent { get; private set; }
    public IReadOnlyCollection<PostComment> Replies => _replies.AsReadOnly();
    
    private PostComment() { /* private constructor for EF */}

    public PostComment(Guid postId, Guid userId, string body, Guid? parentCommentId = null)
    {
        Id = Guid.NewGuid();
        PostId = postId;
        UserId = userId;
        SetBody(body);
        ParentCommentId = parentCommentId;
        SetCreatedDate();
        UpdatedDate = CreatedDate;
    }

    public void Edit(string body)
    {
        SetBody(body);
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    private void SetBody(string body)
    {
        if (string.IsNullOrWhiteSpace(body)) throw new ArgumentException("Body is required.");
        Body = body.Trim();
    }
}