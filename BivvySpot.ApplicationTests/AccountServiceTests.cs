using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Application.Services;
using BivvySpot.Model.Dtos;          // AuthContext, AccountProfileResponse, UpdateAccountProfileRequest
using BivvySpot.Model.Entities;     // User
using Moq;

namespace BivvySpot.ApplicationTests;

public class AccountServiceTests
{
    private readonly Mock<IUserRepository> _repo = new();
    private AccountService CreateSut() => new(_repo.Object);

    [Fact]
    public async Task RegisterOrUpsertAsync_Creates_New_User_When_Not_Found()
    {
        // Arrange
        var auth = new AuthContext(
            Provider: "auth0",
            Subject: "auth0|123",
            Email: "User@Example.com",
            DisplayName: "Kyle"
        );

        _repo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|123", It.IsAny<CancellationToken>()))
             .ReturnsAsync((User?)null);
        _repo.Setup(r => r.FindByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
             .ReturnsAsync((User?)null);

        User? addedUser = null;
        _repo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
             .Callback<User, CancellationToken>((u, _) => addedUser = u)
             .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        var result = await sut.RegisterOrUpsertAsync(auth, CancellationToken.None);

        // Assert
        Assert.NotNull(addedUser);
        Assert.Equal("Kyle", addedUser!.Username);
        Assert.Equal("user@example.com", addedUser.Email); // normalized to lower
        // If your User has these properties:
        // Assert.Equal("auth0", addedUser.AuthProvider);
        // Assert.Equal("auth0|123", addedUser.AuthSubject);

        Assert.Equal(addedUser.Id, result.Id);
        Assert.Equal(addedUser.Username, result.Username);
        Assert.Equal(addedUser.Email, result.Email);

        _repo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        // Create path calls SaveChanges twice in your current implementation:
        // - once in CreateUserAsync
        // - once after light refresh in RegisterOrUpsertAsync
        _repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task RegisterOrUpsertAsync_Updates_Existing_User_By_Identity()
    {
        // Arrange
        var existing = new User("OldName", "", "", "old@example.com");
        var auth = new AuthContext("auth0", "auth0|abc", "new@example.com", "NewName");

        _repo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|abc", It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);

        var sut = CreateSut();

        // Act
        var result = await sut.RegisterOrUpsertAsync(auth, CancellationToken.None);

        // Assert
        Assert.Equal("NewName", existing.Username);             // updated
        Assert.Equal("new@example.com", existing.Email);        // normalized + updated
        _repo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(existing.Id, result.Id);
    }

    [Fact]
    public async Task RegisterOrUpsertAsync_Falls_Back_To_Email_When_No_Subject()
    {
        // Arrange
        var existing = new User("SomeName", "", "", "someone@example.com");
        var auth = new AuthContext(null, null, "someone@example.com", "NewerName");

        _repo.Setup(r => r.FindByEmailAsync("someone@example.com", It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);

        var sut = CreateSut();

        // Act
        var result = await sut.RegisterOrUpsertAsync(auth, CancellationToken.None);

        // Assert
        Assert.Equal("NewerName", existing.Username);
        _repo.Verify(r => r.FindByEmailAsync("someone@example.com", It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.FindByIdentityAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(existing.Id, result.Id);
    }

    [Fact]
    public async Task GetCurrentProfileAsync_Returns_Null_When_Not_Found()
    {
        // Arrange
        var auth = new AuthContext("auth0", "auth0|missing", "missing@example.com", "X");
        _repo.Setup(r => r.FindByIdentityAsync("auth0", "auth0|missing", It.IsAny<CancellationToken>()))
             .ReturnsAsync((User?)null);
        _repo.Setup(r => r.FindByEmailAsync("missing@example.com", It.IsAny<CancellationToken>()))
             .ReturnsAsync((User?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.GetCurrentProfileAsync(auth, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCurrentProfileAsync_Updates_When_Found()
    {
        // Arrange
        var existing = new User("Old", "A", "B", "me@example.com");
        var auth = new AuthContext(null, null, "me@example.com", "Ignored");
        var req  = new UpdateAccountProfileRequest("NewUser", "First", "Last");

        _repo.Setup(r => r.FindByEmailAsync("me@example.com", It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);

        var sut = CreateSut();

        // Act
        await sut.UpdateCurrentProfileAsync(auth, req, CancellationToken.None);

        // Assert
        Assert.Equal("NewUser", existing.Username);
        Assert.Equal("First", existing.FirstName);
        Assert.Equal("Last", existing.LastName);
        _repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCurrentProfileAsync_Throws_When_User_Not_Found()
    {
        // Arrange
        var auth = new AuthContext(null, null, "ghost@example.com", "N/A");
        var req  = new UpdateAccountProfileRequest("U", "F", "L");

        _repo.Setup(r => r.FindByEmailAsync("ghost@example.com", It.IsAny<CancellationToken>()))
             .ReturnsAsync((User?)null);

        var sut = CreateSut();

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            sut.UpdateCurrentProfileAsync(auth, req, CancellationToken.None));
    }
}