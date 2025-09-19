namespace BivvySpot.Model.Entities;

public class PostComment
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Body { get; set; } = null!;
    public Guid? ParentCommentId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Post Post { get; set; } = null!;
    public User User { get; set; } = null!;
    public PostComment? Parent { get; set; }
    public ICollection<PostComment> Replies { get; set; } = new List<PostComment>();
}