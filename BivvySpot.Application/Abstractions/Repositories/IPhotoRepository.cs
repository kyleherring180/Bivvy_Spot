using BivvySpot.Model.Entities;

namespace BivvySpot.Application.Abstractions.Repositories;

public interface IPhotoRepository
{
    Task AddRangeAsync(IEnumerable<PostPhoto> photos, CancellationToken ct);
    Task<List<PostPhoto>> GetForPostAsync(Guid postId, CancellationToken ct);
    Task<int> CountForPostAsync(Guid postId, CancellationToken ct);
    Task<PostPhoto?> GetAsync(Guid photoId, CancellationToken ct);
    Task<bool> PostExistsForAuthorAsync(Guid postId, Guid authorUserId, CancellationToken ct);
    Task RemoveAsync(PostPhoto photo, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}