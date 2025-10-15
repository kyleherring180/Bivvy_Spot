using BivvySpot.Application.Abstractions.Infrastructure;
using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Model.Dtos;
using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;

namespace BivvySpot.Application.Services;

public class LocationService(
    ILocationRepository locationsRepo,
    ILocationSuggestionRepository locationSuggestionsRepo,
    IPostRepository postsRepo,
    IAccountService accountService,
    IGeometryParser geom) : ILocationService
{
    // Create official location (editor/admin)
    public async Task<Location> CreateAsync(CreateLocationDto req, CancellationToken ct)
    {
        ValidateCreate(req);

        var name = req.Name.Trim();
        if (await locationsRepo.ExistsDuplicateAsync(name, req.Type, req.ParentId, ct))
            throw new InvalidOperationException("A location with the same name/alias/type/parent exists.");

        var point = geom.BuildPoint(req.Latitude, req.Longitude);
        if (point is not null && await locationsRepo.ExistsNearbyDuplicateAsync(req.Type, point, 100, ct))
            throw new InvalidOperationException("A similar location exists nearby.");

        var boundary = geom.ParsePolygon(req.BoundaryGeoJson);

        var loc = new Location(name, req.Type, point, boundary, req.CountryCode, req.ParentId, req.Elevation);
        if (req.AltNames is { Count: > 0 })
            loc.SetAltNames(req.AltNames.Select(a => (a.Name, a.Language)));

        await locationsRepo.AddAsync(loc, ct);
        await locationsRepo.SaveChangesAsync(ct);

        return loc;
    }

    // Any user can suggest
    public async Task SuggestAsync(AuthContext auth, CreateLocationSuggestionDto req, CancellationToken ct)
    {
        if (!auth.HasAnyKey) throw new UnauthorizedAccessException();

        ValidateSuggestion(req);

        var point = geom.BuildPoint(req.Latitude, req.Longitude)!;
        if (await locationsRepo.ExistsNearbyDuplicateAsync(req.LocationType, point, 100, ct))
            throw new InvalidOperationException("A similar location likely exists nearby.");
        
        var user = await accountService.GetCurrentProfileAsync(auth, ct) ?? throw new UnauthorizedAccessException("User not found.");

        var s = new LocationSuggestion(user.Id, req.Name.Trim(), req.LocationType, point, req.CountryCode, req.ParentId, req.Note);
        await locationSuggestionsRepo.AddAsync(s, ct);
        await locationSuggestionsRepo.SaveChangesAsync(ct);
    }

    // Moderator approval → create official Location
    public async Task<Location> ApproveSuggestionAsync(Guid suggestionId, CancellationToken ct)
    {
        var s = await locationSuggestionsRepo.GetAsync(suggestionId, ct) ?? throw new KeyNotFoundException("Suggestion not found.");
        if (s.Status != SuggestionStatus.Open) throw new InvalidOperationException("Suggestion not open.");

        var loc = new Location(s.Name, s.LocationType, s.Point, null, s.CountryCode, s.ParentId);
        await locationsRepo.AddAsync(loc, ct);
        s.MarkApproved(loc.Id);

        await locationsRepo.SaveChangesAsync(ct);

        return loc;
    }

    public async Task<List<Location>> SearchAsync(string q, LocationType? type, int limit, CancellationToken ct)
    {
        q = (q ?? string.Empty).Trim();
        if (q.Length == 0) return new List<Location>([]);
        var list = await locationsRepo.SearchByNameOrAliasAsync(q, type, Math.Clamp(limit, 1, 50), ct);

        // Simple projection; if you want to flag exact alias that matched, do that in repo
        return list.ToList();
    }

    // Replace set of locations for a post (order preserved by index)
    public async Task ReplacePostLocationsAsync(Guid postId, IReadOnlyCollection<Guid> locationIds, CancellationToken ct)
    {
        var desired = (locationIds ?? Array.Empty<Guid>()).Where(id => id != Guid.Empty).ToList();
        if (desired.Count == 0)
        {
            // Remove all
            var current = await postsRepo.GetPostLocationIdsAsync(postId, ct);
            foreach (var id in current) await postsRepo.RemoveLocationFromPostAsync(postId, id, ct);
            await postsRepo.SaveChangesAsync(ct);
            return;
        }

        if (!await locationsRepo.AreActiveAsync(desired, ct))
            throw new ArgumentException("One or more locations are invalid or inactive.");

        var currentIds = await postsRepo.GetPostLocationIdsAsync(postId, ct);
        var desiredSet = desired.ToHashSet();

        foreach (var add in desiredSet.Except(currentIds))
            await postsRepo.AddLocationToPostAsync(postId, add, desired.IndexOf(add), ct);

        foreach (var remove in currentIds.Except(desiredSet))
            await postsRepo.RemoveLocationFromPostAsync(postId, remove, ct);

        // update orders for retained
        for (int i = 0; i < desired.Count; i++)
            if (currentIds.Contains(desired[i]))
                await postsRepo.SetLocationOrderAsync(postId, desired[i], i, ct);

        await postsRepo.SaveChangesAsync(ct);
    }

    // --- helpers ---
    private static void ValidateCreate(CreateLocationDto r)
    {
        if (string.IsNullOrWhiteSpace(r.Name)) throw new ArgumentException("Name required");
        if (r.Latitude is null && string.IsNullOrWhiteSpace(r.BoundaryGeoJson))
            throw new ArgumentException("Provide a point (lat/lon) or a boundary.");
        if (r.Latitude is not null && (r.Latitude < -90 || r.Latitude > 90)) throw new ArgumentOutOfRangeException(nameof(r.Latitude));
        if (r.Longitude is not null && (r.Longitude < -180 || r.Longitude > 180)) throw new ArgumentOutOfRangeException(nameof(r.Longitude));
        if (r.CountryCode is { Length: > 2 }) throw new ArgumentException("CountryCode must be ISO-2.");
        if (r.Elevation is < 0) throw new ArgumentOutOfRangeException(nameof(r.Elevation));
        if (r.AltNames is { Count: > 50 }) throw new ArgumentException("Too many alternate names.");
    }

    private static void ValidateSuggestion(CreateLocationSuggestionDto r)
    {
        if (string.IsNullOrWhiteSpace(r.Name)) throw new ArgumentException("Name required");
        // if (r.Point.X < -90 || r.Point.Y> 90) throw new ArgumentOutOfRangeException(nameof(r.Latitude));
        // if (r.Longitude < -180 || r.Longitude > 180) throw new ArgumentOutOfRangeException(nameof(r.Longitude));
        if (r.CountryCode is { Length: > 2 }) throw new ArgumentException("CountryCode must be ISO-2.");
    }

    private static string? MatchAlias(Location l, string q)
        => l.AltNames.FirstOrDefault(a => a.Name.Contains(q, StringComparison.OrdinalIgnoreCase))?.Name;
}