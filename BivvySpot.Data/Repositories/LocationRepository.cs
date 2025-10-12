using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Model.Enums;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Location = BivvySpot.Model.Entities.Location;

namespace BivvySpot.Data.Repositories;

public class LocationRepository(BivvySpotContext dbContext) : ILocationRepository
{
    public Task<Location?> GetAsync(Guid id, CancellationToken ct)
        => dbContext.Locations
            .Include(l => l.AltNames)
            .SingleOrDefaultAsync(l => l.Id == id && l.DeletedDate == null, ct);

    public Task AddAsync(Location loc, CancellationToken ct)
    {
        dbContext.Locations.Add(loc); 
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);

    public async Task<bool> AreActiveAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        var set = ids.ToHashSet();
        var count = await dbContext.Locations.CountAsync(l => set.Contains(l.Id) && l.DeletedDate == null, ct);
        return count == set.Count;
    }

    public Task<bool> ExistsDuplicateAsync(string name, LocationType type, Guid? parentId, CancellationToken ct)
        => dbContext.Locations.AnyAsync(l =>
               l.DeletedDate == null &&
               l.LocationType == type &&
               l.ParentId == parentId &&
               (l.Name == name || l.AltNames.Any(a => a.Name == name)), ct);

    public Task<Location?> FindByNameOrAliasAsync(string name, LocationType type, Guid? parentId, CancellationToken ct)
        => dbContext.Locations
            .Include(l => l.AltNames)
            .Where(l => l.DeletedDate == null && l.LocationType == type && l.ParentId == parentId)
            .Where(l => l.Name == name || l.AltNames.Any(a => a.Name == name))
            .FirstOrDefaultAsync(ct);

    public async Task<bool> ExistsNearbyDuplicateAsync(LocationType type, Point point, double meters, CancellationToken ct)
        => await dbContext.Locations.AnyAsync(l =>
               l.DeletedDate == null &&
               l.LocationType == type &&
               l.Point != null &&
               l.Point!.Distance(point) <= meters, ct); // NTS geography distance (meters if geography)

    public async Task<IReadOnlyList<Location>> SearchByNameOrAliasAsync(string q, LocationType? type, int limit, CancellationToken ct)
        => await dbContext.Locations
            .Include(l => l.AltNames)
            .Where(l => l.DeletedDate == null && (type == null || l.LocationType == type))
            .Where(l => l.Name.Contains(q) || l.AltNames.Any(a => a.Name.Contains(q)))
            .OrderBy(l => l.Name)
            .Take(limit)
            .ToListAsync(ct);
}