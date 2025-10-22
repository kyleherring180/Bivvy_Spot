using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Contracts.v1.Request;
using BivvySpot.Contracts.v1.Response;
using BivvySpot.Presentation.v1.MapToContract;
using BivvySpot.Presentation.v1.MapToModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BivvySpot.Presentation.v1.Controllers;

[ApiController]
[Route("api/v1/difficulties")]
public class DifficultyController(IDifficultyService difficultyService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<DifficultyResponse>> Create([FromBody] CreateDifficultyRequest req, CancellationToken ct)
    {
        var difficulty = await difficultyService.CreateAsync(req.ActivityType.ToModel(), req.DifficultyRating, ct);
        // Returning 200 OK with created resource as there's no GET endpoint yet
        return Ok(difficulty.ToContract());
    }
}
