using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BivvySpot.Data.Repositories;

public class TagRepository(BivvySpotContext dbContext) :ITagRepository
{
    public async Task<Dictionary<string, Tag>> FindBySlugsAsync(IEnumerable<string> slugs, CancellationToken ct)
    {
        var set = slugs.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return await dbContext.Tags
            .Where(t => set.Contains(t.Slug))
            .ToDictionaryAsync(t => t.Slug, t => t, StringComparer.OrdinalIgnoreCase, ct);
    }

    public Task AddAsync(Tag tag, CancellationToken ct)
    {
        dbContext.Tags.Add(tag);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
}