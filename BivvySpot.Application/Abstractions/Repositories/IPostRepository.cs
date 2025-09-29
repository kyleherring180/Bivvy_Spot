using BivvySpot.Model.Entities;

namespace BivvySpot.Application.Abstractions.Repositories;

public interface IPostRepository
{
    Task AddAsync(Post post, CancellationToken ct);
    Task<Post> GetByIdForAuthorAsync(Guid postId, Guid authorUserId, CancellationToken ct);
    Task<Post> GetPostByIdAsync(Guid postId);
    Task<IEnumerable<Post>> GetPostsAsync(int page, int pageSize);
    Task<HashSet<Guid>> GetPostByTagIdsAsync(Guid postId, CancellationToken ct);
    Task AddTagToPostAsync(Guid postId, Guid tagId, CancellationToken ct);
    Task RemoveTagFromPostAsync(Guid postId, Guid tagId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    
}