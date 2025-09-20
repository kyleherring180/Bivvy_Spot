namespace BivvySpot.Model.Entities;

public class PostDifficulty
{
    public Guid Id { get; set; }
    public Guid DifficultyId { get; set; }
    public Guid PostId { get; set; }

    public Difficulty Difficulty { get; set; } = null!;
    public Post Post { get; set; } = null!;
    
    private PostDifficulty() { /* private constructor for EF */}
}