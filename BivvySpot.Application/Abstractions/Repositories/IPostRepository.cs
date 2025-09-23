using BivvySpot.Model.Entities;

namespace BivvySpot.Application.Abstractions.Repositories;

public interface IPostRepository
{
    Task AddAsync(Post post, CancellationToken ct);
    Task<Post> GetByIdForAuthorAsync(Guid postId, Guid authorUserId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}