using BivvySpot.Application.Abstractions.Infrastructure;
using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Application.Uploads;
using BivvySpot.Contracts.v1.Request;
using BivvySpot.Contracts.v1.Response;
using BivvySpot.Model.Dtos;
using BivvySpot.Model.Entities;
using Microsoft.Extensions.Configuration;

namespace BivvySpot.Application.Services;

public class GpxService(
    IUserRepository userRepository,
    IGpxRepository gpxRepository,
    IObjectStorage storage,
    IConfiguration config) : IGpxService
{
    private const long MaxGpxBytes = 50 * 1024 * 1024;
    private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
        { "application/gpx+xml", "application/xml", "text/xml", "application/octet-stream" };
    private readonly string _gpxContainer = config["Storage:Containers:Gpx"] ?? "gpx";

    public async Task<GpxTrackResponse> UploadProxyAsync(AuthContext auth, Guid postId, UploadItem file, CancellationToken ct)
    {
        RequireAuth(auth);
        Validate(file.ContentType, file.Length);

        var user = await RequireUser(auth, ct);
        await RequirePostOwnership(postId, user.Id, ct);
        await EnforceSingleGpx(postId, ct);

        var ext = NormalizeExt(Path.GetExtension(file.FileName));
        var key = $"gpx/{DateTime.UtcNow:yyyy/MM}/{postId}/{Guid.NewGuid():N}{ext}";

        string checksum;
        using (var s = file.OpenReadStream())
        using (var sha = System.Security.Cryptography.SHA256.Create())
        {
            checksum = BitConverter.ToString(sha.ComputeHash(s)).Replace("-", "").ToLowerInvariant();
            s.Position = 0;
            await storage.UploadAsync(_gpxContainer, key, s, "application/gpx+xml", null, ct);
        }

        var entity = new GpxTrack(postId, key, checksum, file.Length);
        await gpxRepository.AddAsync(entity, ct);
        await gpxRepository.SaveChangesAsync(ct);

        return new GpxTrackResponse(entity.Id, entity.StorageKey, entity.PreviewGeoJsonKey, entity.SizeBytes, entity.Distance, entity.AscentM);
    }

    public async Task<GetGpxUploadSasResponse> GetUploadSasAsync(AuthContext auth, Guid postId, GetGpxUploadSasRequest req, CancellationToken ct)
    {
        RequireAuth(auth);
        Validate(req.ContentType, req.ContentLength);

        var user = await RequireUser(auth, ct);
        await RequirePostOwnership(postId, user.Id, ct);
        await EnforceSingleGpx(postId, ct);

        var ext = NormalizeExt(req.Extension);
        var key = $"gpx/{DateTime.UtcNow:yyyy/MM}/{postId}/{Guid.NewGuid():N}{ext}";
        var sas = await storage.GetSasForClientUploadAsync(_gpxContainer, key, TimeSpan.FromMinutes(10), ct);
        return new GetGpxUploadSasResponse(sas, key);
    }

    public async Task<GpxTrackResponse> CompleteUploadAsync(AuthContext auth, Guid postId, CompleteGpxUploadRequest req, CancellationToken ct)
    {
        RequireAuth(auth);
        Validate(req.ContentType, 1);

        var user = await RequireUser(auth, ct);
        await RequirePostOwnership(postId, user.Id, ct);
        await EnforceSingleGpx(postId, ct);

        var entity = new GpxTrack(postId, req.BlobKey, req.ChecksumSha256 ?? "", 0);
        await gpxRepository.AddAsync(entity, ct);
        await gpxRepository.SaveChangesAsync(ct);

        return new GpxTrackResponse(entity.Id, entity.StorageKey, entity.PreviewGeoJsonKey, entity.SizeBytes, entity.Distance, entity.AscentM);
    }
    
    public async Task DeleteAsync(AuthContext auth, Guid postId, Guid gpxId, CancellationToken ct)
    {
        // 1) Auth + ownership
        RequireAuth(auth);
        var user = await RequireUser(auth, ct);
        await RequirePostOwnership(postId, user.Id, ct);

        // 2) Load the post's GPX and verify the id matches
        var existing = await gpxRepository.GetByPostAsync(postId, ct);
        if (existing is null || existing.Id != gpxId)
            throw new KeyNotFoundException("GPX not found for this post.");

        // 3) Optional: delete the blob from storage (uncomment if you want physical deletion now)
        // await storage.DeleteAsync(_gpxContainer, existing.StorageKey, ct);

        // 4) Domain delete + persistence
        existing.SoftDelete();                 // keep audit fields updated
        await gpxRepository.RemoveAsync(existing, ct); // hard delete via repo (or keep row and just SaveChanges for soft-delete)
        await gpxRepository.SaveChangesAsync(ct);
    }

    // helpers
    private static void RequireAuth(AuthContext auth)
    {
        if (!auth.IsValid(out var err)) throw new UnauthorizedAccessException(err);
    }

    private void Validate(string contentType, long len)
    {
        if (len <= 0 || len > MaxGpxBytes) throw new ArgumentException("Invalid GPX size.");
        if (!AllowedTypes.Contains(contentType)) throw new ArgumentException($"Unsupported GPX content type: {contentType}");
    }

    private static string NormalizeExt(string ext)
    {
        if (!string.IsNullOrWhiteSpace(ext)) ext = ext.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(ext)) ext = ".gpx";
        if (!ext.StartsWith('.')) ext = "." + ext;
        return ext;
    }

    private async Task EnforceSingleGpx(Guid postId, CancellationToken ct)
    {
        if (await gpxRepository.HasForPostAsync(postId, ct))
            throw new InvalidOperationException("This post already has a GPX file.");
    }

    private async Task<User> RequireUser(AuthContext auth, CancellationToken ct)
        => await userRepository.FindByIdentityAsync(auth.Provider!, auth.Subject!, ct)
           ?? throw new KeyNotFoundException("Local user not found. Call /account/register first.");

    private async Task RequirePostOwnership(Guid postId, Guid userId, CancellationToken ct)
    {
        if (await gpxRepository.PostExistsForAuthorAsync(postId, userId, ct))
            return;

        throw new UnauthorizedAccessException("Not your post.");
    }
}
