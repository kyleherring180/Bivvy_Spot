using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BivvySpot.Data.Repositories;

public class LocationSuggestionRepository(BivvySpotContext dbContext) : ILocationSuggestionRepository
{
    public Task AddAsync(LocationSuggestion s, CancellationToken ct)
    {
        dbContext.LocationSuggestions.Add(s); 
        return Task.CompletedTask;
    }
    
    public Task<LocationSuggestion?> GetAsync(Guid id, CancellationToken ct)
        => dbContext.LocationSuggestions.SingleOrDefaultAsync(x => x.Id == id, ct);
    
    public Task SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
}