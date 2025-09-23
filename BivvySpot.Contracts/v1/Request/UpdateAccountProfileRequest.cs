namespace BivvySpot.Contracts.v1.Request;

public record UpdateAccountProfileRequest(string? Username, string? FirstName, string? LastName);