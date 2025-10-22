using System.Reflection;
using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Application.Services;
using BivvySpot.Model.Dtos;
using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;
using Moq;

namespace BivvySpot.ApplicationTests;

public class PostServiceTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IPostRepository> _postRepo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();

    private static AuthContext Auth() => new("auth0", "auth0|user", "user@example.com", "User");

    private static User MakeUser() => new("User", "First", "Last", "user@example.com");

    private PostService CreateSut() => new(_userRepo.Object, _postRepo.Object, _tagRepo.Object);
    
    private static Difficulty MakeDifficulty()
        => new(ActivityType.Hiking, "Easy");

    private static Post MakePost(Guid? userId = null)
        => new(userId ?? Guid.NewGuid(), title: "T", body: "B", season: Season.Summer, elevationGain: 10, duration: 20, difficultyId: MakeDifficulty().Id, routeName: null);

    private static void AddInteractionViaReflection(Post post, Interaction interaction)
    {
        var field = typeof(Post).GetField("_interactions", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var list = (List<Interaction>)field.GetValue(post)!;
        list.Add(interaction);
    }

    private static void AddReportViaReflection(Post post, Report report)
    {
        var field = typeof(Post).GetField("_reports", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var list = (List<Report>)field.GetValue(post)!;
        list.Add(report);
    }

    [Fact]
    public async Task CreateAsync_Creates_Post_And_Saves()
    {
        // Arrange
        var author = MakeUser();
        _userRepo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|user", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(author);

        Post? addedPost = null;
        _postRepo.Setup(r => r.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
                 .Callback<Post, CancellationToken>((p, _) => addedPost = p)
                 .Returns(Task.CompletedTask);

        var sut = CreateSut();
        var dto = new CreatePostDto
                {
                    Title = "Title",
                    Body = "Body",
                    Season = Season.Summer,
                    ElevationGain = 100,
                    Duration = 60
                };

        // Act
        var result = await sut.CreateAsync(Auth(), dto, CancellationToken.None);

        // Assert
        Assert.NotNull(addedPost);
        Assert.Equal(addedPost!.Id, result.Id);
        _postRepo.Verify(r => r.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Once);
        _postRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Throws_If_Not_Author()
    {
        // Arrange
        var author = MakeUser();
        _userRepo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|user", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(author);
        _postRepo.Setup(r => r.GetByIdForAuthorAsync(It.IsAny<Guid>(), author.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Post)null!);

        var sut = CreateSut();

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            sut.UpdateAsync(Auth(), Guid.NewGuid(), new UpdatePostDto(), CancellationToken.None));
    }

    [Fact]
    public async Task GetPostByIdAsync_Throws_When_NotFound()
    {
        _postRepo.Setup(r => r.GetPostByIdAsync(It.IsAny<Guid>()))
                 .ReturnsAsync((Post)null!);
        var sut = CreateSut();
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetPostByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task AddInteractionAsync_Adds_And_Increments_Count()
    {
        // Arrange
        var user = MakeUser();
        _userRepo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|user", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);
        var post = MakePost();
        _postRepo.Setup(r => r.GetPostByIdAsync(post.Id)).ReturnsAsync(post);

        Interaction? added = null;
        _postRepo.Setup(r => r.AddInteractionAsync(It.IsAny<Interaction>(), It.IsAny<CancellationToken>()))
                 .Callback<Interaction, CancellationToken>((i, _) => added = i)
                 .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        var result = await sut.AddInteractionAsync(Auth(), post.Id, InteractionType.Like, CancellationToken.None);

        // Assert
        Assert.Same(post, result);
        Assert.Equal(1, result.LikeCount);
        Assert.NotNull(added);
        Assert.Equal(user.Id, added!.UserId);
        Assert.Equal(post.Id, added.PostId);
        Assert.Equal(InteractionType.Like, added.InteractionType);
        _postRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddInteractionAsync_Does_Not_Duplicate_When_Already_Exists()
    {
        // Arrange
        var user = MakeUser();
        _userRepo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|user", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);
        var post = MakePost();
        AddInteractionViaReflection(post, new Interaction(user.Id, post.Id, InteractionType.Like));
        _postRepo.Setup(r => r.GetPostByIdAsync(post.Id)).ReturnsAsync(post);

        var sut = CreateSut();

        // Act
        var result = await sut.AddInteractionAsync(Auth(), post.Id, InteractionType.Like, CancellationToken.None);

        // Assert: no add, no save, counts unchanged
        Assert.Same(post, result);
        Assert.Equal(0, result.LikeCount);
        _postRepo.Verify(r => r.AddInteractionAsync(It.IsAny<Interaction>(), It.IsAny<CancellationToken>()), Times.Never);
        _postRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveInteractionAsync_Removes_And_Decrements_Count()
    {
        var user = MakeUser();
        _userRepo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|user", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);
        var post = MakePost();
        // start with 1 like so decrement goes to 0
        post.ApplyInteractionChange(InteractionType.Like, +1);
        _postRepo.Setup(r => r.GetPostByIdAsync(post.Id)).ReturnsAsync(post);

        _postRepo.Setup(r => r.RemoveInteractionAsync(user.Id, post.Id, InteractionType.Like, It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        var sut = CreateSut();

        var result = await sut.RemoveInteractionAsync(Auth(), post.Id, InteractionType.Like, CancellationToken.None);

        Assert.Same(post, result);
        Assert.Equal(0, result.LikeCount);
        _postRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReportPostAsync_Creates_Report_When_None_Open()
    {
        var user = MakeUser();
        _userRepo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|user", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);
        var post = MakePost();
        _postRepo.Setup(r => r.GetPostByIdAsync(post.Id)).ReturnsAsync(post);

        Report? added = null;
        _postRepo.Setup(r => r.AddReportAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
                 .Callback<Report, CancellationToken>((rep, _) => added = rep)
                 .Returns(Task.CompletedTask);

        var sut = CreateSut();

        var report = await sut.ReportPostAsync(Auth(), post.Id, "Spam", CancellationToken.None);

        Assert.NotNull(added);
        Assert.Equal(added!.Id, report.Id);
        _postRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReportPostAsync_Throws_When_Already_Open_By_User()
    {
        var user = MakeUser();
        _userRepo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|user", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);
        var post = MakePost();
        AddReportViaReflection(post, new Report(post.Id, user.Id, "Spam"));
        _postRepo.Setup(r => r.GetPostByIdAsync(post.Id)).ReturnsAsync(post);

        var sut = CreateSut();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.ReportPostAsync(Auth(), post.Id, "Spam again", CancellationToken.None));
    }

    [Fact]
    public async Task ModerateReportAsync_Updates_Report_Via_Domain_Method()
    {
        var moderator = MakeUser();
        _userRepo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|user", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(moderator);
        var post = MakePost();
        var report = new Report(post.Id, Guid.NewGuid(), "Spam");
        AddReportViaReflection(post, report);
        _postRepo.Setup(r => r.GetPostByIdAsync(post.Id)).ReturnsAsync(post);

        var sut = CreateSut();

        var updated = await sut.ModerateReportAsync(Auth(), post.Id, report.Id, ReportStatus.Resolved, "Handled", CancellationToken.None);

        Assert.Equal(ReportStatus.Resolved, updated.Status);
        Assert.Equal("Handled", updated.ModeratorNote);
        Assert.Equal(moderator.Id, updated.ResolvedBy);
        Assert.NotNull(updated.ResolvedDate);
        _postRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
