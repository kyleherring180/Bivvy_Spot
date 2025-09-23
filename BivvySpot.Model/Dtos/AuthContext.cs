using System.Security.Claims;

namespace BivvySpot.Model.Dtos;

// Auth context built from Auth0 JWT
public record AuthContext(string? Provider, string? Subject, string? Email, string? DisplayName)
{
    public bool HasAnyKey => !string.IsNullOrWhiteSpace(Subject) && !string.IsNullOrWhiteSpace(Provider);

    public bool IsValid(out string error)
    {
        if (!HasAnyKey) { error = "Token missing 'sub' claim."; return false; }
        error = string.Empty; return true;
    }

    public static AuthContext From(ClaimsPrincipal user)
    {
        var sub = user.FindFirst("sub")?.Value;
        var iss = user.FindFirst("iss")?.Value ?? "";
        var provider = !string.IsNullOrEmpty(iss) && iss.Contains("auth0", StringComparison.OrdinalIgnoreCase)
            ? "auth0" : "external";
        var email = user.FindFirst("email")?.Value?.Trim().ToLowerInvariant();
        var name = (user.Identity?.Name ?? email ?? $"user-{Guid.NewGuid():N}").Trim();
        return new AuthContext(provider, sub, email, name);
    }
}