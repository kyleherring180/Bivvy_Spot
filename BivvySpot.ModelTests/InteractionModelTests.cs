using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;

namespace BivvySpot.ModelTests;

public class InteractionModelTests
{
    [Fact]
    public void Constructor_Sets_Fields()
    {
        var userId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        var i = new Interaction(userId, postId, InteractionType.Save);
        Assert.Equal(userId, i.UserId);
        Assert.Equal(postId, i.PostId);
        Assert.Equal(InteractionType.Save, i.InteractionType);
        Assert.True(i.CreatedDate <= DateTimeOffset.UtcNow);
    }
}
