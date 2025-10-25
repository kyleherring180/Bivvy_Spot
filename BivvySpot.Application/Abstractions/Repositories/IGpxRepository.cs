using BivvySpot.Model.Entities;

namespace BivvySpot.Application.Abstractions.Repositories;

public interface IGpxRepository
{
    Task AddAsync(GpxTrack track, CancellationToken ct);
    Task<GpxTrack?> GetByPostAsync(Guid postId, CancellationToken ct);
    Task<bool> HasForPostAsync(Guid postId, CancellationToken ct);
    Task<bool> PostExistsForAuthorAsync(Guid postId, Guid authorUserId, CancellationToken ct);
    Task RemoveAsync(GpxTrack track, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}