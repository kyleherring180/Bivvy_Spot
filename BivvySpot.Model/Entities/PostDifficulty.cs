namespace BivvySpot.Model.Entities;

public class PostDifficulty
{
    public Guid Id { get; private set; }
    public Guid DifficultyId { get; private set; }
    public Guid PostId { get; private set; }

    public Difficulty Difficulty { get; private set; } = null!;
    public Post Post { get; private set; } = null!;
    
    private PostDifficulty() { /* private constructor for EF */}

    public PostDifficulty(Guid postId, Guid difficultyId)
    {
        Id = Guid.NewGuid();
        PostId = postId;
        DifficultyId = difficultyId;
    }
}