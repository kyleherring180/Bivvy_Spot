using BivvySpot.Model.Dtos;
using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;

namespace BivvySpot.Application.Abstractions.Services;

public interface ILocationService
{
    Task<Location> CreateAsync(CreateLocationDto req, CancellationToken ct);
    Task SuggestAsync(AuthContext auth, CreateLocationSuggestionDto req, CancellationToken ct);
    Task<Location> ApproveSuggestionAsync(Guid suggestionId, CancellationToken ct);
    Task<List<Location>> SearchAsync(string q, LocationType? type, int limit, CancellationToken ct);
    Task ReplacePostLocationsAsync(Guid postId, IReadOnlyCollection<Guid> locationIds, CancellationToken ct);
}