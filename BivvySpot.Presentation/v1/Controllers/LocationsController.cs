using BivvySpot.Application.Abstractions.Repositories;
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
[Route("api/v1/locations")]
public class LocationsController(ILocationService locationService, IAuthContextProvider authContextProvider) : ControllerBase
{
    // Admin/editor create official location
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<LocationResponse>> Create([FromBody] CreateLocationRequest req, CancellationToken ct)
    {
        var res = await locationService.CreateAsync(req.ToDto(), ct);
        return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<LocationResponse>> GetById(Guid id, [FromServices] ILocationRepository repo, CancellationToken ct)
    {
        var loc = await repo.GetAsync(id, ct);
        return loc is null ? NotFound() : Ok(loc.ToResponse());
    }

    // Search: name or alias (+ optional type)
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<LocationResponse>>> Search(
        [FromQuery] string q, [FromQuery] Contracts.Shared.LocationType? type, [FromQuery] int limit = 20,
        CancellationToken ct = default)
    {
        var result = await locationService.SearchAsync(q, type?.ToModel(), limit, ct);
        
        return Ok(result.Select(r => r.ToResponse()).ToList());
    } 
    
    // Admin/editor create official location
    [HttpPost("ApproveSuggestion/{id:guid}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<LocationResponse>> ApproveSuggestion(Guid id, CancellationToken ct)
    {
        var res = await locationService.ApproveSuggestionAsync(id, ct);
        return Ok(res.ToResponse());
    }

    [HttpPost("suggestions")]
    [Authorize]
    public async Task<IActionResult> Suggest([FromBody] CreateLocationSuggestionRequest req, CancellationToken ct)
    {
        await locationService.SuggestAsync(authContextProvider.GetCurrent(), req.ToDto(), ct);
        return Ok("Location Suggestion received");
    }
}