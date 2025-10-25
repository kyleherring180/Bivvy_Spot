using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BivvySpot.Data.Repositories;

public class GpxRepository(BivvySpotContext db) : IGpxRepository
{
    public Task AddAsync(GpxTrack track, CancellationToken ct)
    { db.GpxTracks.Add(track); return Task.CompletedTask; }

    public Task<GpxTrack?> GetByPostAsync(Guid postId, CancellationToken ct)
        => db.GpxTracks.SingleOrDefaultAsync(g => g.PostId == postId && g.DeletedDate == null, ct);

    public Task<bool> HasForPostAsync(Guid postId, CancellationToken ct)
        => db.GpxTracks.AnyAsync(g => g.PostId == postId && g.DeletedDate == null, ct);

    public Task<bool> PostExistsForAuthorAsync(Guid postId, Guid authorUserId, CancellationToken ct)
        => db.Posts.AnyAsync(p => p.Id == postId && p.UserId == authorUserId && p.DeletedDate == null, ct);

    public Task RemoveAsync(GpxTrack track, CancellationToken ct)
    { db.GpxTracks.Remove(track); return Task.CompletedTask; }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}