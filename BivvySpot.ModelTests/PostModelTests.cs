using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;

namespace BivvySpot.ModelTests;

public class PostModelTests
{
    [Fact]
    public void Constructor_Sets_Defaults()
    {
        var userId = Guid.NewGuid();
        var difficultyId = Guid.NewGuid();
        var post = new Post(userId, "  Title  ", "Body", Season.Summer, 10, 5, difficultyId);
        Assert.Equal(userId, post.UserId);
        Assert.Equal("Title", post.Title);
        Assert.Equal(0, post.LikeCount);
        Assert.Equal(0, post.SaveCount);
        Assert.Equal(PostStatus.Draft, post.Status);
        Assert.True(post.CreatedDate <= post.UpdatedDate);
    }

    [Fact]
    public void Update_Changes_Fields_And_Normalizes()
    {
        var post = new Post(Guid.NewGuid(), "T", "B", Season.Winter, 0, 0, Guid.NewGuid());
        post.Update(title: "  New  ", routeName: "  Route  ", body: "NB", season: Season.Summer, elevationGain: 123, duration: 45, status: PostStatus.Published);
        Assert.Equal("New", post.Title);
        Assert.Equal("Route", post.RouteName);
        Assert.Equal("NB", post.Body);
        Assert.Equal(Season.Summer, post.Season);
        Assert.Equal(123, post.ElevationGain);
        Assert.Equal(45, post.Duration);
        Assert.Equal(PostStatus.Published, post.Status);
    }

    [Fact]
    public void ApplyInteractionChange_Updates_Counts_And_Clamps()
    {
        var post = new Post(Guid.NewGuid(), "T", "B", Season.Summer, 0, 0, Guid.NewGuid());
        post.ApplyInteractionChange(InteractionType.Like, +1);
        post.ApplyInteractionChange(InteractionType.Save, +2);
        Assert.Equal(1, post.LikeCount);
        Assert.Equal(2, post.SaveCount);

        post.ApplyInteractionChange(InteractionType.Like, -5);
        post.ApplyInteractionChange(InteractionType.Save, -1);
        Assert.Equal(0, post.LikeCount); // clamped
        Assert.Equal(1, post.SaveCount);
    }
}
