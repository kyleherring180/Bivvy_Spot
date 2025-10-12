using BivvySpot.Model.Enums;
using NetTopologySuite.Geometries;
using Location = BivvySpot.Model.Entities.Location;

namespace BivvySpot.Application.Abstractions.Repositories;

public interface ILocationRepository
{
    Task<bool> ExistsDuplicateAsync(string name, LocationType type, Guid? parentId, CancellationToken ct);
    Task<bool> ExistsNearbyDuplicateAsync(LocationType type, Point point, double meters, CancellationToken ct);
    Task<Location?> FindByNameOrAliasAsync(string name, LocationType type, Guid? parentId, CancellationToken ct);
    Task<IReadOnlyList<Location>> SearchByNameOrAliasAsync(string q, LocationType? type, int limit, CancellationToken ct);

    Task<Location?> GetAsync(Guid id, CancellationToken ct);
    Task AddAsync(Location loc, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<bool> AreActiveAsync(IEnumerable<Guid> ids, CancellationToken ct); // DeletedDate == null
}