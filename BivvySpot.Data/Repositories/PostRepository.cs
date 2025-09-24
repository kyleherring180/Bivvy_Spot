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
    

    public Task SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
}