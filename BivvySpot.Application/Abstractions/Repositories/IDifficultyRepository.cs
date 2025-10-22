using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;

namespace BivvySpot.Application.Abstractions.Repositories;

public interface IDifficultyRepository
{
    Task<Difficulty?> FindAsync(ActivityType activityType, string difficultyRating, CancellationToken ct);
    Task AddAsync(Difficulty difficulty, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}