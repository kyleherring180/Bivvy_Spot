using BivvySpot.Application.Abstractions.Security;
using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Application.Uploads;
using BivvySpot.Contracts.v1.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BivvySpot.Presentation.v1.Controllers;

[ApiController]
[Route("api/v1/posts/{postId:guid}/gpx")]
public sealed class PostGpxController(
    IGpxService gpxService,
    IAuthContextProvider authContextProvider) : ControllerBase
{
    // Proxy upload (single GPX)
    [HttpPost]
    [Authorize]
    [RequestSizeLimit(60_000_000)]
    public Task<IActionResult> Upload(Guid postId, IFormFile file, CancellationToken ct)
    {
        var authContext = authContextProvider.GetCurrent();
        var item = new UploadItem(file.FileName, file.ContentType, file.Length, () => file.OpenReadStream());
        return gpxService.UploadProxyAsync(authContext, postId, item, ct)
                  .ContinueWith<IActionResult>(t => Created($"/api/v1/posts/{postId}/gpx/{t.Result.Id}", t.Result), ct);
    }

    [HttpPost("sas")]
    [Authorize]
    public Task<IActionResult> GetSas(Guid postId, [FromBody] GetGpxUploadSasRequest req, CancellationToken ct)
    {
        var authContext = authContextProvider.GetCurrent();
        return gpxService.GetUploadSasAsync(authContext, postId, req, ct)
                  .ContinueWith<IActionResult>(t => Ok(t.Result), ct);
    }

    [HttpPost("complete")]
    [Authorize]
    public Task<IActionResult> Complete(Guid postId, [FromBody] CompleteGpxUploadRequest req, CancellationToken ct)
    {
        var authContext = authContextProvider.GetCurrent();
        return gpxService.CompleteUploadAsync(authContext, postId, req, ct)
                  .ContinueWith<IActionResult>(t => Ok(t.Result), ct);
    }

    [HttpDelete("{gpxId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid postId, Guid gpxId, CancellationToken ct)
    {
        var authContext = authContextProvider.GetCurrent();
        await gpxService.DeleteAsync(authContext, postId, gpxId, ct);
        return NoContent();
    }
}
