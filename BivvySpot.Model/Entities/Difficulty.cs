using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Entities;

public class Difficulty
{
    private readonly List<PostDifficulty> _postDifficulties = new List<PostDifficulty>();
    public Guid Id { get; private set; }
    public ActivityType ActivityType { get; private set; }
    public string DifficultyRating { get; private set; } = null!;
    public IReadOnlyCollection<PostDifficulty> PostDifficulties => _postDifficulties.AsReadOnly();
    
    private Difficulty() { /* private constructor for EF */}

    public Difficulty(ActivityType activityType, string difficultyRating)
    {
        Id = Guid.NewGuid();
        ActivityType = activityType;
        DifficultyRating = difficultyRating.Trim();
    }
}