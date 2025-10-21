using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;

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
    Task<HashSet<Guid>> GetPostLocationIdsAsync(Guid postId, CancellationToken ct);
    Task AddLocationToPostAsync(Guid postId, Guid locationId, int order, CancellationToken ct);
    Task RemoveLocationFromPostAsync(Guid postId, Guid locationId, CancellationToken ct);
    Task SetLocationOrderAsync(Guid postId, Guid locationId, int order, CancellationToken ct);
    // Interactions
    Task<bool> HasInteractionAsync(Guid userId, Guid postId, InteractionType type, CancellationToken ct);
    Task AddInteractionAsync(Interaction interaction, CancellationToken ct);
    Task RemoveInteractionAsync(Guid userId, Guid postId, InteractionType type, CancellationToken ct);
    Task<int> GetInteractionCountAsync(Guid postId, InteractionType type, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    
}