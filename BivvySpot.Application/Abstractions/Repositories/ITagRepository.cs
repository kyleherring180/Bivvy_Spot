using BivvySpot.Model.Entities;

namespace BivvySpot.Application.Abstractions.Repositories;

public interface ITagRepository
{
    Task<Dictionary<string, Tag>> FindBySlugsAsync(IEnumerable<string> slugs, CancellationToken ct);
    Task AddAsync(Tag tag, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}