using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;
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
        dbContext.Posts
            .Include(p => p.Interactions)
            .Include(p => p.Reports)
            .Include(p => p.PostTags)
            .Include(p => p.PostLocations)
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(p => p.Id == postId && p.UserId == authorUserId && p.DeletedDate == null, ct)!;

    public Task<Post> GetPostByIdAsync(Guid postId) => 
        dbContext.Posts
            .Include(p => p.Interactions)
            .Include(p => p.Reports)
            .Include(p => p.PostTags)
            .Include(p => p.PostLocations)
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(p => p.Id == postId)!;
    
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

    public async Task<HashSet<Guid>> GetPostLocationIdsAsync(Guid postId, CancellationToken ct) =>
        (await dbContext.PostLocations.Where(x => x.PostId == postId)
            .Select(x => x.LocationId).ToListAsync(ct)).ToHashSet();

    public Task AddLocationToPostAsync(Guid postId, Guid locationId, int order, CancellationToken ct)
    {
        dbContext.PostLocations.Add(new PostLocation(postId, locationId, order));
        return Task.CompletedTask;
    }

    public async Task RemoveLocationFromPostAsync(Guid postId, Guid locationId, CancellationToken ct)
    {
        var link = await dbContext.PostLocations.SingleOrDefaultAsync(x => x.PostId == postId && x.LocationId == locationId, ct);
        if (link is not null) dbContext.PostLocations.Remove(link);
    }

    public async Task SetLocationOrderAsync(Guid postId, Guid locationId, int order, CancellationToken ct)
    {
        var existing = await dbContext.PostLocations.SingleOrDefaultAsync(x => x.PostId == postId && x.LocationId == locationId, ct);
        if (existing is not null)
        {
            dbContext.PostLocations.Remove(existing);
        }
        dbContext.PostLocations.Add(new PostLocation(postId, locationId, Math.Max(0, order)));
    }

    public Task AddInteractionAsync(Interaction interaction, CancellationToken ct)
    {
        dbContext.Interactions.Add(interaction);
        return Task.CompletedTask;
    }

    public async Task RemoveInteractionAsync(Guid userId, Guid postId, InteractionType type, CancellationToken ct)
    {
        var entity = await dbContext.Interactions.SingleOrDefaultAsync(i => i.UserId == userId && i.PostId == postId && i.InteractionType == type, ct);
        if (entity is not null) dbContext.Interactions.Remove(entity);
    }

    public Task AddReportAsync(Report report, CancellationToken ct)
    {
        dbContext.Reports.Add(report);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
}