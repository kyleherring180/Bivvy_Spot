using System.Reflection;
using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Application.Services;
using BivvySpot.Model.Dtos;
using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;
using Moq;

namespace BivvySpot.ApplicationTests;

public class PostServiceCommentsTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IPostRepository> _postRepo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();

    private static AuthContext Auth() => new("auth0", "auth0|user", "user@example.com", "User");
    private static User MakeUser() => new("User", "First", "Last", "user@example.com");
    private PostService CreateSut() => new(_userRepo.Object, _postRepo.Object, _tagRepo.Object);
    private static Post MakePost(Guid? userId = null)
        => new(userId ?? Guid.NewGuid(), title: "T", body: "B", season: Season.Summer, elevationGain: 10, duration: 20, routeName: null);

    private static void AddTopLevelComment(Post post, PostComment c)
    {
        var field = typeof(Post).GetField("_comments", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var list = (List<PostComment>)field.GetValue(post)!;
        list.Add(c);
    }

    [Fact]
    public async Task AddCommentAsync_Adds_TopLevel_Comment()
    {
        var user = MakeUser();
        var post = MakePost();
        _userRepo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|user", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _postRepo.Setup(r => r.GetPostByIdAsync(post.Id)).ReturnsAsync(post);

        PostComment? added = null;
        _postRepo.Setup(r => r.AddCommentAsync(It.IsAny<PostComment>(), It.IsAny<CancellationToken>()))
                 .Callback<PostComment, CancellationToken>((c, _) => added = c)
                 .Returns(Task.CompletedTask);

        var sut = CreateSut();
        var result = await sut.AddCommentAsync(Auth(), post.Id, " Hello ", null, CancellationToken.None);

        Assert.NotNull(added);
        Assert.Equal(added!.Id, result.Id);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(post.Id, result.PostId);
        Assert.Null(result.ParentCommentId);
        _postRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_Allows_Reply_To_TopLevel_Only()
    {
        var user = MakeUser();
        var post = MakePost();
        var top = new PostComment(post.Id, Guid.NewGuid(), "Top");
        AddTopLevelComment(post, top);
        var reply = new PostComment(post.Id, Guid.NewGuid(), "reply", top.Id);
        // add reply into top.Replies via reflection (since EF would populate)
        var repliesField = typeof(PostComment).GetField("_replies", BindingFlags.Instance | BindingFlags.NonPublic)!;
        ((List<PostComment>)repliesField.GetValue(top)!).Add(reply);

        _userRepo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|user", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _postRepo.Setup(r => r.GetPostByIdAsync(post.Id)).ReturnsAsync(post);

        var sut = CreateSut();
        // Attempt to reply to a reply should fail
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.AddCommentAsync(Auth(), post.Id, "nope", reply.Id, CancellationToken.None));
    }

    [Fact]
    public async Task EditCommentAsync_Updates_When_Owner()
    {
        var user = MakeUser();
        var post = MakePost();
        var c = new PostComment(post.Id, user.Id, "Old");
        AddTopLevelComment(post, c);

        _userRepo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|user", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _postRepo.Setup(r => r.GetPostByIdAsync(post.Id)).ReturnsAsync(post);

        var sut = CreateSut();
        var updated = await sut.EditCommentAsync(Auth(), post.Id, c.Id, "New", CancellationToken.None);

        Assert.Equal("New", updated.Body);
        _postRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EditCommentAsync_Throws_When_Not_Owner()
    {
        var user = MakeUser();
        var post = MakePost();
        var c = new PostComment(post.Id, Guid.NewGuid(), "Old");
        AddTopLevelComment(post, c);

        _userRepo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|user", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _postRepo.Setup(r => r.GetPostByIdAsync(post.Id)).ReturnsAsync(post);

        var sut = CreateSut();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            sut.EditCommentAsync(Auth(), post.Id, c.Id, "New", CancellationToken.None));
    }
}
