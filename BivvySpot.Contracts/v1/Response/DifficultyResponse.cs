using BivvySpot.Contracts.Shared;

namespace BivvySpot.Contracts.v1.Response;

public record DifficultyResponse(
    Guid Id,
    ActivityType ActivityType,
    string DifficultyRating
);
