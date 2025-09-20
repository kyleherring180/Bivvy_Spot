using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Entities;

public class Difficulty
{
    private readonly List<PostDifficulty> _postDifficulties = new List<PostDifficulty>();
    public Guid Id { get; set; }
    public ActivityType ActivityType { get; set; }
    public string DifficultyRating { get; set; } = null!;
    public IReadOnlyCollection<PostDifficulty> PostDifficulties => _postDifficulties.AsReadOnly();
    
    private Difficulty() { /* private constructor for EF */}
}