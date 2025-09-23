namespace BivvySpot.Contracts.v1.Response;

public record AccountProfileResponse(Guid Id, string? Username, string? Email, string? FirstName, string? LastName);