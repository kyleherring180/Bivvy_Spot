using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BivvySpot.Data.Repositories;

public class PostRepository(BivvySpotContext dbContext) : IPostRepository
{
    public Task AddAsync(Post post, CancellationToken ct)
    {
        dbContext.Posts.Add(post); 
        return Task.CompletedTask;
    }

    public Task<Post> GetByIdForAuthorAsync(Guid postId, Guid authorUserId, CancellationToken ct) =>
        dbContext.Posts.SingleOrDefaultAsync(p => p.Id == postId && p.UserId == authorUserId && p.DeletedDate == null, ct)!;

    public Task<Post> GetPostByIdAsync(Guid postId) => 
        dbContext.Posts.SingleOrDefaultAsync(p => p.Id == postId)!;
    
    public Task<IEnumerable<Post>> GetPostsAsync(int page, int pageSize) =>
    dbContext.PostPhotos
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync()
        .ContinueWith(t => (IEnumerable<Post>)t.Result);

    public async Task<HashSet<Guid>> GetPostByTagIdsAsync(Guid postId, CancellationToken ct) =>
        (await dbContext.PostTags.Where(x => x.PostId == postId)
            .Select(x => x.TagId).ToListAsync(ct)).ToHashSet();

    public Task AddTagToPostAsync(Guid postId, Guid tagId, CancellationToken ct)
    {
        dbContext.PostTags.Add(new PostTag(postId, tagId));
        return Task.CompletedTask;
    }

    public async Task RemoveTagFromPostAsync(Guid postId, Guid tagId, CancellationToken ct)
    {
        var link = await dbContext.PostTags.SingleOrDefaultAsync(x => x.PostId == postId && x.TagId == tagId, ct);
        if (link is not null) dbContext.PostTags.Remove(link);
    }

    public Task SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
}