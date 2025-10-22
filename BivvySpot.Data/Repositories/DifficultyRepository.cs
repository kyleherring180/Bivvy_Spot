using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace BivvySpot.Data.Repositories;

public class DifficultyRepository(BivvySpotContext dbContext) : IDifficultyRepository
{
    public Task<Difficulty?> FindAsync(ActivityType activityType, string difficultyRating, CancellationToken ct)
        => dbContext.Difficulties.SingleOrDefaultAsync(d => d.ActivityType == activityType && d.DifficultyRating == difficultyRating, ct);

    public Task AddAsync(Difficulty difficulty, CancellationToken ct)
    {
        dbContext.Difficulties.Add(difficulty);
        return Task.CompletedTask;
    }
    
    public Task SaveChangesAsync(CancellationToken ct)
        => dbContext.SaveChangesAsync(ct);
}