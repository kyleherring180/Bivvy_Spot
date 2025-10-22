using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;

namespace BivvySpot.Application.Abstractions.Services;

public interface IDifficultyService
{
    Task<Difficulty> CreateAsync(ActivityType activityType, string difficultyRating, CancellationToken ct);
}