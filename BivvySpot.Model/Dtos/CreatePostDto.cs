using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Dtos;

public class CreatePostDto
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public Season Season { get; set; }
    public int ElevationGain { get; set; }
    public int Duration { get; set; }
    public string? RouteName { get; set; }
}