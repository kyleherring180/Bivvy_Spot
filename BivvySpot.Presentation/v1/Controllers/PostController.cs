using BivvySpot.Application.Abstractions.Security;
using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Contracts.v1.Request;
using BivvySpot.Contracts.v1.Response;
using BivvySpot.Model.Enums;
using BivvySpot.Presentation.v1.MapToContract;
using BivvySpot.Presentation.v1.MapToModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BivvySpot.Presentation.v1.Controllers;

[ApiController]
[Route("api/v1/posts")]
public class PostController(IPostService postService, IAuthContextProvider authContextProvider) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PostResponse>> Create([FromBody] CreatePostRequest req, CancellationToken ct)
    {
        var result = await postService.CreateAsync(authContextProvider.GetCurrent(), req.ToDto(), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToContract());
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<PostResponse>> Update(Guid id, [FromBody] UpdatePostRequest req, CancellationToken ct)
    {
        var result = await postService.UpdateAsync(authContextProvider.GetCurrent(), id, req.ToDto(), ct);
        return Ok(result.ToContract());
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostResponse>> GetById(Guid id)
    {
        var result = await postService.GetPostByIdAsync(id);
        return Ok(result.ToContract());
    }
    
    [HttpGet()]
    public async Task<ActionResult<PostResponse>> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var results = await postService.GetPostsAsync(page, pageSize);
        return Ok(results.Select(r => r.ToContract()));
    }

    // Interactions
    [HttpPost("{id:guid}/interactions")]
    [Authorize]
    public async Task<IActionResult> AddInteraction(Guid id, [FromBody] AddInteractionRequest req, CancellationToken ct)
    {
        await postService.AddInteractionAsync(authContextProvider.GetCurrent(), id, req.InteractionType, ct);
        return Ok($"Interaction {req.InteractionType} added to post {id}");
    }

    [HttpDelete("{id:guid}/interactions")]
    [Authorize]
    public async Task<IActionResult> RemoveInteraction(Guid id, [FromBody] AddInteractionRequest req, CancellationToken ct)
    {
        await postService.RemoveInteractionAsync(authContextProvider.GetCurrent(), id, req.InteractionType, ct);
        return Ok($"Interaction {req.InteractionType} removed from post {id}");
    }

    // Comments
    [HttpPost("{id:guid}/comments")]
    [Authorize]
    public async Task<IActionResult> AddComment(Guid id, [FromBody] AddCommentRequest req, CancellationToken ct)
    {
        var comment = await postService.AddCommentAsync(authContextProvider.GetCurrent(), id, req.Body, req.ParentCommentId, ct);
        return Ok(new { commentId = comment.Id });
    }

    [HttpPut("{postId:guid}/comments/{commentId:guid}")]
    [Authorize]
    public async Task<IActionResult> EditComment(Guid postId, Guid commentId, [FromBody] EditCommentRequest req, CancellationToken ct)
    {
        await postService.EditCommentAsync(authContextProvider.GetCurrent(), postId, commentId, req.Body, ct);
        return Ok($"Comment {commentId} updated on post {postId}");
    }

    // Reports
    [HttpPost("{id:guid}/reports")]
    [Authorize]
    public async Task<IActionResult> ReportPost(Guid id, [FromBody] ReportPostRequest req, CancellationToken ct)
    {
        await postService.ReportPostAsync(authContextProvider.GetCurrent(), id, req.Reason, ct);
        return Ok($"Report submitted for post {id}");
    }

    [HttpPut("{postId:guid}/reports/{reportId:guid}")]
    [Authorize]
    public async Task<IActionResult> ModerateReport(Guid postId, Guid reportId, [FromBody] ModerateReportRequest req, CancellationToken ct)
    {
        await postService.ModerateReportAsync(authContextProvider.GetCurrent(), postId, reportId, req.Status, req.ModeratorNote, ct);
        return Ok($"Report {reportId} for post {postId} moderated to {req.Status}");
    }
    
}
