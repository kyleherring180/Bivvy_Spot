namespace BivvySpot.Model.Dtos;

public record AccountProfileResponse(Guid Id, string Username, string Email, string? FirstName, string? LastName);