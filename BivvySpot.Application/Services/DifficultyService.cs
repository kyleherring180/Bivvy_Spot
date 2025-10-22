using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;

namespace BivvySpot.Application.Services;

public class DifficultyService(IDifficultyRepository difficultyRepository) : IDifficultyService
{
    public async Task<Difficulty> CreateAsync(ActivityType activityType, string difficultyRating, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(difficultyRating)) throw new ArgumentException("Difficulty rating is required.");
        difficultyRating = difficultyRating.Trim();

        var existing = await difficultyRepository.FindAsync(activityType, difficultyRating, ct);
        if (existing is not null) return existing;

        var difficulty = new Difficulty(activityType, difficultyRating);
        await difficultyRepository.AddAsync(difficulty, ct);
        await difficultyRepository.SaveChangesAsync(ct);
        return difficulty;
    }
}