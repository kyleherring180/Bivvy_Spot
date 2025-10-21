namespace BivvySpot.Contracts.v1.Request;

public class AddCommentRequest
{
    public string Body { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
}
