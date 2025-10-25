using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BivvySpot.Data.Repositories;

public class PhotoRepository(BivvySpotContext db) : IPhotoRepository
{
    public Task AddRangeAsync(IEnumerable<PostPhoto> photosToAdd, CancellationToken ct)
    { db.PostPhotos.AddRange(photosToAdd); return Task.CompletedTask; }

    public Task<List<PostPhoto>> GetForPostAsync(Guid postId, CancellationToken ct)
        => db.PostPhotos.Where(p => p.PostId == postId && p.DeletedDate == null).OrderBy(p => p.SortOrder).ToListAsync(ct);

    public Task<int> CountForPostAsync(Guid postId, CancellationToken ct)
        => db.PostPhotos.CountAsync(p => p.PostId == postId && p.DeletedDate == null, ct);

    public Task<PostPhoto?> GetAsync(Guid photoId, CancellationToken ct)
        => db.PostPhotos.SingleOrDefaultAsync(p => p.Id == photoId && p.DeletedDate == null, ct);

    public Task<bool> PostExistsForAuthorAsync(Guid postId, Guid authorUserId, CancellationToken ct)
        => db.Posts.AnyAsync(p => p.Id == postId && p.UserId == authorUserId && p.DeletedDate == null, ct);

    public Task RemoveAsync(PostPhoto photo, CancellationToken ct)
    { db.PostPhotos.Remove(photo); return Task.CompletedTask; }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}