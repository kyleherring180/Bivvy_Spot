using BivvySpot.Application.Abstractions.Security;
using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Contracts.v1.Request;
using BivvySpot.Contracts.v1.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BivvySpot.Presentation.v1.Controllers;

[ApiController]
[Route("api/v1/account")]
public class AccountController(IAccountService accountService, IAuthContextProvider authContextProvider) : ControllerBase
{
    [HttpPost("register")]
    [Authorize]
    public async Task<ActionResult<AccountProfileResponse>> Register(CancellationToken ct)
    {
        var auth = authContextProvider.GetCurrent();
        if (!auth.IsValid(out var err)) return BadRequest(new { message = err });

        var profile = await accountService.RegisterOrUpsertAsync(auth, ct);
        return Ok(profile);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<AccountProfileResponse>> GetProfile(CancellationToken ct)
    {
        var auth = authContextProvider.GetCurrent();
        var profile = await accountService.GetCurrentProfileAsync(auth, ct);
        return profile is null
            ? NotFound(new { message = "Not registered locally. Call POST /account/register." })
            : Ok(profile);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateAccountProfileRequest req, CancellationToken ct)
    {
        var auth = authContextProvider.GetCurrent();
        await accountService.UpdateCurrentProfileAsync(auth, req, ct);
        return NoContent();
    }
}

