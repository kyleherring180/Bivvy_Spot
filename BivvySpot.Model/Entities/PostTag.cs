namespace BivvySpot.Model.Entities;

public class PostTag
{
    public Guid Id { get; set; }
    public Tag Tag { get; set; }
    public Post Post { get; set; }
}