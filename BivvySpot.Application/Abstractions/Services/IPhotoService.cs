using BivvySpot.Application.Uploads;
using BivvySpot.Contracts.v1.Request;
using BivvySpot.Contracts.v1.Response;
using BivvySpot.Model.Dtos;

namespace BivvySpot.Application.Abstractions.Services;

public interface IPhotoService
{
    Task<PostPhotoBatchResponse> UploadProxyBatchAsync(AuthContext auth, Guid postId, IReadOnlyList<UploadItem> files, CancellationToken ct);
    Task<GetUploadSasBatchResponse> GetUploadSasBatchAsync(AuthContext auth, Guid postId, GetPhotoUploadSasBatchRequest req, CancellationToken ct);
    Task<PostPhotoBatchResponse> CompleteUploadBatchAsync(AuthContext auth, Guid postId, CompletePhotoUploadBatchRequest req, CancellationToken ct);
    Task ReorderAsync(AuthContext auth, Guid postId, IReadOnlyList<Guid> orderedPhotoIds, CancellationToken ct);
    Task DeleteAsync(AuthContext auth, Guid postId, Guid photoId, CancellationToken ct);
}