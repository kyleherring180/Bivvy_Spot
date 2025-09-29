using BivvySpot.Contracts.Shared;
using BivvySpot.Contracts.v1.Response;

namespace BivvySpot.Presentation.v1.MapToContract;

public static class PostResponseExtensions
{
    public static PostResponse ToContract(this Model.Entities.Post post)
    {
        return new PostResponse(
                post.Id,
                post.UserId,
                post.User.Username,
                post.Title,
                post.RouteName,
                post.Body,
                post.Season.ToContract(),
                post.ElevationGain,
                post.Duration,
                post.Status.ToContract(),
                post.UpdatedDate,
                post.PostTags.ToContract()
                );
    }
    
    public static Season ToContract(this Model.Enums.Season season) => season switch
    {
        Model.Enums.Season.Summer => Season.Summer,
        Model.Enums.Season.Winter => Season.Winter,
        Model.Enums.Season.MultiSeason => Season.MultiSeason,
        Model.Enums.Season.Unknown => Season.Unknown,
        _ => throw new ArgumentOutOfRangeException(nameof(season), $"Not expected season value: {season}"),
    };
    
    public static PostStatus ToContract(this Model.Enums.PostStatus status) => status switch
    {
        Model.Enums.PostStatus.Draft => PostStatus.Draft,
        Model.Enums.PostStatus.Published => PostStatus.Published,
        Model.Enums.PostStatus.Hidden => PostStatus.Hidden,
        Model.Enums.PostStatus.Flagged => PostStatus.Flagged,
        _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected post status value: {status}"),
    };
    
    public static IReadOnlyCollection<string> ToContract(this IEnumerable<Model.Entities.PostTag> postTags)
    {
        return postTags.Select(pt => pt.Tag.Name).ToList();
    }
}