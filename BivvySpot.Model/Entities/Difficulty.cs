using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Entities;

public class Difficulty
{
    public Guid Id { get; set; }
    public ActivityType ActivityType { get; set; }
    public string DifficultyRating { get; set; } = null!;
}