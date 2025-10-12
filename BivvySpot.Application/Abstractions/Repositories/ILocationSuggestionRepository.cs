using BivvySpot.Model.Entities;

namespace BivvySpot.Application.Abstractions.Repositories;

public interface ILocationSuggestionRepository
{
    Task AddAsync(LocationSuggestion s, CancellationToken ct);
    Task<LocationSuggestion?> GetAsync(Guid id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}