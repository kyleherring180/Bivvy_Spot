using BivvySpot.Contracts.v1.Request;
using BivvySpot.Model.Dtos;
using BivvySpot.Model.Entities;

namespace BivvySpot.Application.Abstractions.Services;

public interface IPostService
{
    Task<Post> CreateAsync(AuthContext auth, CreatePostDto dto, CancellationToken ct);
    Task<Post> UpdateAsync(AuthContext auth, Guid postId, UpdatePostDto dto, CancellationToken ct);
    Task<Post> GetPostByIdAsync(Guid postId);
}