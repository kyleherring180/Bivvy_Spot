using BivvySpot.Model.Dtos;
using BivvySpot.Model.Enums;

namespace BivvySpot.Presentation.v1.MapToModel;

public static class PostExtensions
{
    public static CreatePostDto ToDto(this Contracts.v1.Request.CreatePostRequest req)
    {
        return new CreatePostDto
        {
            Title = req.Title,
            RouteName = req.RouteName,
            Body = req.Body,
            Season = req.Season.ToModel(),
            ElevationGain = req.ElevationGain,
            Duration = req.Duration,
            Tags = req.Tags?.ToList() ?? new List<string>(),
            LocationIds = req.LocationIds?.ToList(),
            DifficultyId = req.DifficultyId
        };
    }
    
    public static UpdatePostDto ToDto(this Contracts.v1.Request.UpdatePostRequest req)
    {
        return new UpdatePostDto
        {
            Title = req.Title,
            RouteName = req.RouteName,
            Body = req.Body,
            Season = req.Season?.ToModel(),
            ElevationGain = req.ElevationGain,
            Duration = req.Duration,
            Status = req.Status?.ToModel(),
            Tags = req.Tags?.ToList() ?? new List<string>(),
            RowVersion = req.RowVersion,
            LocationIds = req.LocationIds?.ToList()
        };
    }
    
    public static Season ToModel(this Contracts.Shared.Season season) => season switch
    {
        Contracts.Shared.Season.Summer => Season.Summer,
        Contracts.Shared.Season.Winter => Season.Winter,
        Contracts.Shared.Season.MultiSeason => Season.MultiSeason,
        Contracts.Shared.Season.Unknown => Season.Unknown,
        _ => throw new ArgumentOutOfRangeException(nameof(season), $"Not expected season value: {season}"),
    };
    
    public static PostStatus ToModel(this Contracts.Shared.PostStatus status) => status switch
    {
        Contracts.Shared.PostStatus.Draft => PostStatus.Draft,
        Contracts.Shared.PostStatus.Published => PostStatus.Published,
        Contracts.Shared.PostStatus.Hidden => PostStatus.Hidden,
        Contracts.Shared.PostStatus.Flagged => PostStatus.Flagged,
        _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected post status value: {status}"),
    };
}