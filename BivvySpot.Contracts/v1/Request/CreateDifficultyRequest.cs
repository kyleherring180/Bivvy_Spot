using BivvySpot.Contracts.Shared;

namespace BivvySpot.Contracts.v1.Request;

public record CreateDifficultyRequest(ActivityType ActivityType, string DifficultyRating);
