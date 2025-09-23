using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Dtos;

public class UpdatePostDto
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public Season? Season { get; set; }
    public int? ElevationGain { get; set; }
    public int? Duration { get; set; }
    public string? RouteName { get; set; }
    public PostStatus? Status { get; set; }
    public byte[]? RowVersion { get; set; }
}