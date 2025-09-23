using System.Security.Claims;
using BivvySpot.Application.Abstractions.Security;
using BivvySpot.Model.Dtos;
using Microsoft.AspNetCore.Http;

namespace BivvySpot.Presentation.Security;

public sealed class AuthContextProvider(IHttpContextAccessor http) : IAuthContextProvider
{
    public AuthContext GetCurrent()
    {
        var user = http.HttpContext?.User ?? new ClaimsPrincipal();

        // Keep raw claim names: ensure MapInboundClaims=false in JwtBearer setup
        var sub = user.FindFirst("sub")?.Value
                  ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var iss = user.FindFirst("iss")?.Value ?? string.Empty;
        var provider = iss.Contains("auth0", StringComparison.OrdinalIgnoreCase) ? "auth0" : "external";

        // Email is optional; may come from a namespaced claim if you added an Auth0 Action
        var email = user.FindFirst("email")?.Value
                    ?? user.FindFirst(ClaimTypes.Email)?.Value
                    ?? user.FindFirst("https://bivvyspot/email")?.Value;

        email = email?.Trim().ToLowerInvariant();
        var name = (user.Identity?.Name ?? email ?? $"user-{Guid.NewGuid():N}").Trim();

        return new AuthContext(provider, sub, email, name);
    }
}