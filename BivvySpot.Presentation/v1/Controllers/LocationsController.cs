using BivvySpot.Application.Abstractions.Repositories;
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
public class LocationsController(ILocationService svc) : ControllerBase
{
    // Admin/editor create official location
    [HttpPost]
    [Authorize(/* Roles = "editor,admin" */)]
    public async Task<ActionResult<LocationResponse>> Create([FromBody] CreateLocationRequest req, CancellationToken ct)
    {
        var res = await svc.CreateAsync(req.ToDto(), ct);
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
        var result = await svc.SearchAsync(q, type?.ToModel(), limit, ct);
        
        return Ok(result.Select(r => r.ToResponse()).ToList());
    } 
}