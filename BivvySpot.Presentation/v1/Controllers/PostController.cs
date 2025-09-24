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
        var result = await postService.CreateAsync(authContextProvider.GetCurrent(), req.ToModel(), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToContract());
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<PostResponse>> Update(Guid id, [FromBody] UpdatePostRequest req, CancellationToken ct)
    {
        var result = await postService.UpdateAsync(authContextProvider.GetCurrent(), id, req.ToModel(), ct);
        return Ok(result.ToContract());
    }

    // add a read method later if you like
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostResponse>> GetById(Guid id)
    {
        var result = await postService.GetPostByIdAsync(id);
        return Ok(result.ToContract());
    }
}
