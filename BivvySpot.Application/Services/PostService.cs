using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Model.Dtos;
using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BivvySpot.Application.Services;

public class PostService(
    IUserRepository userRepository,
    IPostRepository postRepository,
    ITagRepository tagRepository
    ) : IPostService
{
    public async Task<Post> CreateAsync(AuthContext auth, CreatePostDto dto, CancellationToken ct)
    {
        var author = await RequireUserAsync(auth, ct);
        ValidateCreate(dto);

        var post = new Post(
            userId: author.Id,
            title: dto.Title,
            body: dto.Body,
            season: dto.Season,
            elevationGain: dto.ElevationGain,
            duration: dto.Duration,
            routeName: dto.RouteName
        );

        await postRepository.AddAsync(post, ct);
        
        if (dto.Tags is { Count: > 0 })
            await UpsertAndLinkTagsAsync(post.Id, dto.Tags, ct);
        
        await postRepository.SaveChangesAsync(ct);
        return post;
    }

    public async Task<Post> UpdateAsync(AuthContext auth, Guid postId, UpdatePostDto dto, CancellationToken ct)
    {
        var author = await RequireUserAsync(auth, ct);
        var post = await postRepository.GetByIdForAuthorAsync(postId, author.Id, ct)
                   ?? throw new KeyNotFoundException("Post not found or not owned by the current user.");

        // (Optional) optimistic concurrency with RowVersion if you expose it on BaseEntity.
        // if (req.RowVersion is not null) _db.Entry(post).Property("RowVersion").OriginalValue = req.RowVersion;

        ValidateUpdate(dto);
        post.Update(dto.Title, dto.RouteName, dto.Body, dto.Season, dto.ElevationGain, dto.Duration, dto.Status);

        await postRepository.SaveChangesAsync(ct);
        return post;
    }

    public async Task<Post> GetPostByIdAsync(Guid postId)
        => await postRepository.GetPostByIdAsync(postId)
        ?? throw new KeyNotFoundException("Post not found.");
    
    public async Task<IEnumerable<Post>> GetPostsAsync(int page, int pageSize)
        => await postRepository.GetPostsAsync(page, pageSize);

    private async Task<User> RequireUserAsync(AuthContext auth, CancellationToken ct)
        => await userRepository.FindByIdentityAsync(auth.Provider!, auth.Subject!, ct)
           ?? throw new KeyNotFoundException("Local user not found. Call /account/register first.");

    private static void ValidateCreate(CreatePostDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title)) throw new ArgumentException("Title is required.");
        if (string.IsNullOrWhiteSpace(dto.Body))  throw new ArgumentException("Body is required.");
        if (dto.ElevationGain < 0) throw new ArgumentException("ElevationGain cannot be negative.");
        if (dto.Duration < 0)      throw new ArgumentException("Duration cannot be negative.");
    }

    private static void ValidateUpdate(UpdatePostDto dto)
    {
        if (dto.ElevationGain is { } eg && eg < 0) throw new ArgumentException("ElevationGain cannot be negative.");
        if (dto.Duration is { } d && d < 0)        throw new ArgumentException("Duration cannot be negative.");
    }
    
    private async Task UpsertAndLinkTagsAsync(Guid postId, IReadOnlyCollection<string> rawNames, CancellationToken ct)
    {
        var norm = Tag.NormalizeTags(rawNames); // slug -> (name, slug)
        if (norm.Count == 0) return;

        // 1) Load existing tags by slug
        var existingBySlug = await tagRepository.FindBySlugsAsync(norm.Keys, ct);

        // 2) Create missing tags (handle race by unique slug)
        foreach (var (slug, pair) in norm)
        {
            if (existingBySlug.ContainsKey(slug)) continue;

            var tag = new Tag(pair.name); // ctor sets Slug via SlugUtil
            try
            {
                await tagRepository.AddAsync(tag, ct);
                existingBySlug[slug] = tag;
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                var retry = await tagRepository.FindBySlugsAsync(new[] { slug }, ct);
                if (retry.TryGetValue(slug, out var t)) existingBySlug[slug] = t; else throw;
            }
        }

        // 3) Compute set delta using IDs
        var desiredIds = existingBySlug.Values.Select(t => t.Id).ToHashSet();
        var currentIds = await postRepository.GetPostByTagIdsAsync(postId, ct);

        foreach (var addId in desiredIds.Except(currentIds))
            await postRepository.AddTagToPostAsync(postId, addId, ct);
        // No removals on create (merge semantics)
    }

    private async Task ReplaceTagsAsync(Guid postId, IReadOnlyCollection<string> rawNames, CancellationToken ct)
    {
        var norm = Tag.NormalizeTags(rawNames); // slug -> (name, slug)
        var existingBySlug = await tagRepository.FindBySlugsAsync(norm.Keys, ct);

        foreach (var (slug, pair) in norm)
        {
            if (existingBySlug.ContainsKey(slug)) continue;

            var tag = new Tag(pair.name);
            try
            {
                await tagRepository.AddAsync(tag, ct);
                existingBySlug[slug] = tag;
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                var retry = await tagRepository.FindBySlugsAsync(new[] { slug }, ct);
                if (retry.TryGetValue(slug, out var t)) existingBySlug[slug] = t; else throw;
            }
        }

        var desiredIds = existingBySlug.Values.Select(t => t.Id).ToHashSet();
        var currentIds = await postRepository.GetPostByTagIdsAsync(postId, ct);

        foreach (var addId in desiredIds.Except(currentIds))
            await postRepository.AddTagToPostAsync(postId, addId, ct);

        foreach (var removeId in currentIds.Except(desiredIds))
            await postRepository.RemoveTagFromPostAsync(postId, removeId, ct);
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        var msg = ex.GetBaseException().Message;
        return msg.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase)
               || msg.Contains("2601") // Cannot insert duplicate key row in object with unique index
               || msg.Contains("2627"); // Violation of UNIQUE KEY constraint
    }
}