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

public class PhotoService(
    IUserRepository userRepository,
    IPhotoRepository photosRepository,
    IObjectStorage storage,
    IConfiguration config) : IPhotoService
{
    private const int  MaxPhotosPerPost = 10;
    private const long MaxPhotoBytes    = 20 * 1024 * 1024;
    private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png" };
    private readonly string _photosContainer = config["Storage:Containers:Photos"] ?? "photos";

    public async Task<PostPhotoBatchResponse> UploadProxyBatchAsync(AuthContext auth, Guid postId, IReadOnlyList<UploadItem> files, CancellationToken ct)
    {
        RequireAuth(auth);
        if (files is null || files.Count == 0) throw new ArgumentException("No files.");
        ValidateAll(files.Select(f => (f.ContentType, f.Length, f.FileName)));

        var user = await RequireUser(auth, ct);
        await RequirePostOwnership(postId, user.Id, ct);

        var existingCount = await photosRepository.CountForPostAsync(postId, ct);
        if (existingCount + files.Count > MaxPhotosPerPost)
            throw new InvalidOperationException($"Photo limit exceeded. Max {MaxPhotosPerPost} per post.");

        var startOrder = await NextOrder(postId, ct);
        var created = new List<PostPhotoResponse>(files.Count);
        var entities = new List<PostPhoto>(files.Count);

        int i = 0;
        foreach (var file in files)
        {
            var ext = NormalizeExt(Path.GetExtension(file.FileName), file.ContentType);
            var key = $"photos/{DateTime.UtcNow:yyyy/MM}/{postId}/{Guid.NewGuid():N}{ext}";

            string checksum;
            using (var s = file.OpenReadStream())
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                checksum = BitConverter.ToString(sha.ComputeHash(s)).Replace("-", "").ToLowerInvariant();
                s.Position = 0;
                await storage.UploadAsync(_photosContainer, key, s, file.ContentType, "public,max-age=31536000,immutable", ct);
            }

            var entity = new PostPhoto(postId, key, file.ContentType, checksum, startOrder + i++);
            entities.Add(entity);

            var url = storage.GetReadUri(_photosContainer, entity.StorageKey).ToString();
            created.Add(new PostPhotoResponse(entity.Id, entity.StorageKey, url, entity.SortOrder, entity.Width, entity.Height));
        }

        await photosRepository.AddRangeAsync(entities, ct);
        await photosRepository.SaveChangesAsync(ct);
        return new PostPhotoBatchResponse(created);
    }

    public async Task<GetUploadSasBatchResponse> GetUploadSasBatchAsync(AuthContext auth, Guid postId, GetPhotoUploadSasBatchRequest req, CancellationToken ct)
    {
        RequireAuth(auth);
        if (req?.Items is null || req.Items.Count == 0) throw new ArgumentException("No items.");
        ValidateAll(req.Items.Select(i => (i.ContentType, i.ContentLength, i.Extension)));

        var user = await RequireUser(auth, ct);
        await RequirePostOwnership(postId, user.Id, ct);

        var existingCount = await photosRepository.CountForPostAsync(postId, ct);
        if (existingCount + req.Items.Count > MaxPhotosPerPost)
            throw new InvalidOperationException($"Photo limit exceeded. Max {MaxPhotosPerPost} per post.");

        var list = new List<GetUploadSasResponse>(req.Items.Count);
        foreach (var it in req.Items)
        {
            var ext = NormalizeExt(it.Extension, it.ContentType);
            var key = $"photos/{DateTime.UtcNow:yyyy/MM}/{postId}/{Guid.NewGuid():N}{ext}";
            var sas = await storage.GetSasForClientUploadAsync(_photosContainer, key, TimeSpan.FromMinutes(10), ct);
            list.Add(new GetUploadSasResponse(sas, key));
        }

        return new GetUploadSasBatchResponse(list);
    }

    public async Task<PostPhotoBatchResponse> CompleteUploadBatchAsync(AuthContext auth, Guid postId, CompletePhotoUploadBatchRequest req, CancellationToken ct)
    {
        RequireAuth(auth);
        if (req?.Items is null || req.Items.Count == 0) throw new ArgumentException("No items.");
        foreach (var it in req.Items)
            if (!AllowedTypes.Contains(it.ContentType)) throw new ArgumentException($"Unsupported image format: {it.ContentType}");

        var user = await RequireUser(auth, ct);
        await RequirePostOwnership(postId, user.Id, ct);

        var existingCount = await photosRepository.CountForPostAsync(postId, ct);
        if (existingCount + req.Items.Count > MaxPhotosPerPost)
            throw new InvalidOperationException($"Photo limit exceeded. Max {MaxPhotosPerPost} per post.");

        var startOrder = await NextOrder(postId, ct);
        var created = new List<PostPhotoResponse>(req.Items.Count);
        var entities = new List<PostPhoto>(req.Items.Count);

        int i = 0;
        foreach (var it in req.Items)
        {
            var entity = new PostPhoto(postId, it.BlobKey, it.ContentType, it.ChecksumSha256 ?? "", startOrder + i++);
            entities.Add(entity);

            var url = storage.GetReadUri(_photosContainer, entity.StorageKey).ToString();
            created.Add(new PostPhotoResponse(entity.Id, entity.StorageKey, url, entity.SortOrder, entity.Width, entity.Height));
        }

        await photosRepository.AddRangeAsync(entities, ct);
        await photosRepository.SaveChangesAsync(ct);
        return new PostPhotoBatchResponse(created);
    }

    public async Task ReorderAsync(AuthContext auth, Guid postId, IReadOnlyList<Guid> orderedPhotoIds, CancellationToken ct)
    {
        RequireAuth(auth);
        var user = await RequireUser(auth, ct);
        await RequirePostOwnership(postId, user.Id, ct);

        var list = await photosRepository.GetForPostAsync(postId, ct);
        var map = orderedPhotoIds.Select((id, idx) => (id, idx)).ToDictionary(x => x.id, x => x.idx);
        foreach (var p in list)
            if (map.TryGetValue(p.Id, out var idx)) p.SetOrder(idx);

        await photosRepository.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(AuthContext auth, Guid postId, Guid photoId, CancellationToken ct)
    {
        RequireAuth(auth);
        var user = await RequireUser(auth, ct);
        await RequirePostOwnership(postId, user.Id, ct);

        var photo = await photosRepository.GetAsync(photoId, ct) ?? throw new KeyNotFoundException("Photo not found.");
        if (photo.PostId != postId) throw new InvalidOperationException("Photo does not belong to post.");

        photo.SoftDelete();
        await photosRepository.RemoveAsync(photo, ct);
        await photosRepository.SaveChangesAsync(ct);
    }

    // helpers
    private static void RequireAuth(AuthContext auth)
    {
        if (!auth.IsValid(out var err)) throw new UnauthorizedAccessException(err);
    }

    private void ValidateAll(IEnumerable<(string ContentType, long Length, string Name)> items)
    {
        foreach (var (type, len, name) in items)
        {
            if (len <= 0 || len > MaxPhotoBytes) throw new ArgumentException($"Invalid size for {name}.");
            if (!AllowedTypes.Contains(type)) throw new ArgumentException($"Unsupported image format: {type}");
        }
    }

    private async Task<int> NextOrder(Guid postId, CancellationToken ct)
        => (await photosRepository.GetForPostAsync(postId, ct)).Select(p => p.SortOrder).DefaultIfEmpty(-1).Max() + 1;

    private static string NormalizeExt(string ext, string contentType)
    {
        if (!string.IsNullOrWhiteSpace(ext)) ext = ext.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(ext))
            ext = contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase) ? ".png" : ".jpg";
        if (!ext.StartsWith('.')) ext = "." + ext;
        return ext is ".jpeg" ? ".jpg" : ext;
    }

    private async Task<User> RequireUser(AuthContext auth, CancellationToken ct)
        => await userRepository.FindByIdentityAsync(auth.Provider!, auth.Subject!, ct)
           ?? throw new KeyNotFoundException("Local user not found. Call /account/register first.");

    private async Task RequirePostOwnership(Guid postId, Guid userId, CancellationToken ct)
    {
        if (await photosRepository.PostExistsForAuthorAsync(postId, userId, ct))
            return;

        throw new UnauthorizedAccessException("Not your post.");
    }
}
