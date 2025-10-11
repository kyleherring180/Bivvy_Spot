using BivvySpot.Application.Abstractions.Security;
using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Contracts.v1.Request;
using BivvySpot.Contracts.v1.Response;
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
    
}
