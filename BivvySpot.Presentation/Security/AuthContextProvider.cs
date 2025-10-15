using System.Security.Claims;
using BivvySpot.Application.Abstractions.Security;
using BivvySpot.Model.Dtos;
using Microsoft.AspNetCore.Http;

namespace BivvySpot.Presentation.Security;

public sealed class AuthContextProvider(IHttpContextAccessor http) : IAuthContextProvider
{
    public AuthContext GetCurrent()
    {
        // Defensive: ensure we have a principal
        var user = http.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
            return new AuthContext(null, null, null, $"user-{Guid.NewGuid():N}");

        // sub (required to identify the user across logins)
        var sub = user.FindFirst("sub")?.Value
                  ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // issuer → detect provider (Auth0 vs external)
        var iss = user.FindFirst("iss")?.Value ?? string.Empty;
        var provider = iss.Contains("auth0", StringComparison.OrdinalIgnoreCase) ? "auth0" : "external";

        // EMAIL: prefer the namespaced claim you added via Auth0 Action
        var email =
            user.FindFirst("https://bivvyspot.io/email")?.Value   // <-- make sure your Action uses this exact key
            ?? user.FindFirst("email")?.Value
            ?? user.FindFirst(ClaimTypes.Email)?.Value;

        email = email?.Trim().ToLowerInvariant();

        // Friendly display name
        var name = (user.FindFirst("name")?.Value
                    ?? user.Identity?.Name
                    ?? email
                    ?? $"user-{Guid.NewGuid():N}")
            .Trim();

        return new AuthContext(provider, sub, email, name);
    }
}