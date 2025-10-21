using BivvySpot.Contracts.v1.Request;
using BivvySpot.Model.Dtos;
using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;

namespace BivvySpot.Application.Abstractions.Services;

public interface IPostService
{
    Task<Post> CreateAsync(AuthContext auth, CreatePostDto dto, CancellationToken ct);
    Task<Post> UpdateAsync(AuthContext auth, Guid postId, UpdatePostDto dto, CancellationToken ct);
    Task<Post> GetPostByIdAsync(Guid postId);
    Task<IEnumerable<Post>> GetPostsAsync(int page, int pageSize);
    Task<Post> AddInteractionAsync(AuthContext auth, Guid postId, InteractionType type, CancellationToken ct);
    Task<Post> RemoveInteractionAsync(AuthContext auth, Guid postId, InteractionType type, CancellationToken ct);
    // Reports
    Task<Report> ReportPostAsync(AuthContext auth, Guid postId, string reason, CancellationToken ct);
    Task<Report> ModerateReportAsync(AuthContext auth, Guid postId, Guid reportId, ReportStatus status, string? moderatorNote, CancellationToken ct);
}