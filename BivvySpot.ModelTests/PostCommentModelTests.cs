using BivvySpot.Model.Entities;

namespace BivvySpot.ModelTests;

public class PostCommentModelTests
{
    [Fact]
    public void Constructor_Sets_Defaults()
    {
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var c = new PostComment(postId, userId, " Hello world ");
        Assert.Equal(postId, c.PostId);
        Assert.Equal(userId, c.UserId);
        Assert.Equal("Hello world", c.Body);
        Assert.True(c.CreatedDate <= c.UpdatedDate);
        Assert.Null(c.ParentCommentId);
    }

    [Fact]
    public void Edit_Updates_Body_And_Timestamp()
    {
        var c = new PostComment(Guid.NewGuid(), Guid.NewGuid(), "Old");
        var before = c.UpdatedDate;
        c.Edit("New Body");
        Assert.Equal("New Body", c.Body);
        Assert.True(c.UpdatedDate >= before);
    }

    [Fact]
    public void Constructor_Throws_For_Empty_Body()
    {
        Assert.Throws<ArgumentException>(() => new PostComment(Guid.NewGuid(), Guid.NewGuid(), "  "));
    }
}
