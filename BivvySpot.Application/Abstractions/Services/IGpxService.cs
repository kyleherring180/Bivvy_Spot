using BivvySpot.Application.Uploads;
using BivvySpot.Contracts.v1.Request;
using BivvySpot.Contracts.v1.Response;
using BivvySpot.Model.Dtos;

namespace BivvySpot.Application.Abstractions.Services;

public interface IGpxService
{
    Task<GpxTrackResponse> UploadProxyAsync(AuthContext auth, Guid postId, UploadItem file, CancellationToken ct);
    Task<GetGpxUploadSasResponse> GetUploadSasAsync(AuthContext auth, Guid postId, GetGpxUploadSasRequest req, CancellationToken ct);
    Task<GpxTrackResponse> CompleteUploadAsync(AuthContext auth, Guid postId, CompleteGpxUploadRequest req, CancellationToken ct);
    Task DeleteAsync(AuthContext auth, Guid postId, Guid gpxId, CancellationToken ct);
}