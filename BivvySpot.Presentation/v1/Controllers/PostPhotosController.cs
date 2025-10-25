using BivvySpot.Application.Abstractions.Security;
using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Application.Uploads;
using BivvySpot.Contracts.v1.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BivvySpot.Presentation.v1.Controllers;

[ApiController]
[Route("api/v1/posts/{postId:guid}/photos")]
public class PostPhotosController(
    IPhotoService photosService,
    IAuthContextProvider authContextProvider) : ControllerBase
{
    // Proxy batch (multipart/form-data, multiple files)
    [HttpPost("batch")]
    [Authorize]
    [RequestSizeLimit(100_000_000)]
    public async Task<IActionResult> UploadBatch(Guid postId, [FromForm] IFormFileCollection files, CancellationToken ct)
    {
        var authContext = authContextProvider.GetCurrent();
        var items = files.Select(f => new UploadItem(f.FileName, f.ContentType, f.Length, () => f.OpenReadStream()))
                         .ToList();
        var created = await photosService.UploadProxyBatchAsync(authContext, postId, items, ct);
        return Created($"/api/v1/posts/{postId}/photos", created);
    }

    // SAS in batch (client uploads directly to blob)
    [HttpPost("sas/batch")]
    [Authorize]
    public Task<IActionResult> GetSasBatch(Guid postId,
        [FromBody] GetPhotoUploadSasBatchRequest req, CancellationToken ct)
    {
        var authContext = authContextProvider.GetCurrent();
        return photosService.GetUploadSasBatchAsync(authContext, postId, req, ct)
            .ContinueWith<IActionResult>(t => Ok(t.Result), ct);
    }

    // Finalize SAS batch
    [HttpPost("complete/batch")]
    [Authorize]
    public Task<IActionResult> CompleteBatch(Guid postId,
        [FromBody] CompletePhotoUploadBatchRequest req, CancellationToken ct)
    {
        var authContext = authContextProvider.GetCurrent();
        return photosService.CompleteUploadBatchAsync(authContext, postId, req, ct)
            .ContinueWith<IActionResult>(t => Ok(t.Result), ct);
    }

    [HttpPatch("order")]
    [Authorize]
    public async Task<IActionResult> Reorder(Guid postId, [FromBody] IReadOnlyList<Guid> orderedPhotoIds, CancellationToken ct)
    {
        var authContext = authContextProvider.GetCurrent();
        await photosService.ReorderAsync(authContext, postId, orderedPhotoIds, ct);
        return NoContent();
    }

    [HttpDelete("{photoId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid postId, Guid photoId, CancellationToken ct)
    {
        var authContext = authContextProvider.GetCurrent();
        await photosService.DeleteAsync(authContext, postId, photoId, ct);
        return NoContent();
    }
}
