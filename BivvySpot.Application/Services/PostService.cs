using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Model.Dtos;
using BivvySpot.Model.Entities;

namespace BivvySpot.Application.Services;

public class PostService(IUserRepository users, IPostRepository posts) : IPostService
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

        await posts.AddAsync(post, ct);
        await posts.SaveChangesAsync(ct);
        return post;
    }

    public async Task<Post> UpdateAsync(AuthContext auth, Guid postId, UpdatePostDto dto, CancellationToken ct)
    {
        var author = await RequireUserAsync(auth, ct);
        var post = await posts.GetByIdForAuthorAsync(postId, author.Id, ct)
                   ?? throw new KeyNotFoundException("Post not found or not owned by the current user.");

        // (Optional) optimistic concurrency with RowVersion if you expose it on BaseEntity.
        // if (req.RowVersion is not null) _db.Entry(post).Property("RowVersion").OriginalValue = req.RowVersion;

        ValidateUpdate(dto);
        post.Update(dto.Title, dto.RouteName, dto.Body, dto.Season, dto.ElevationGain, dto.Duration, dto.Status);

        await posts.SaveChangesAsync(ct);
        return post;
    }

    private async Task<User> RequireUserAsync(AuthContext auth, CancellationToken ct)
        => await users.FindByIdentityAsync(auth.Provider!, auth.Subject!, ct)
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
}