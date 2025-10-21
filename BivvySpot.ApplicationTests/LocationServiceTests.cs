using BivvySpot.Application.Abstractions.Infrastructure;
using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Application.Services;
using BivvySpot.Contracts.v1.Response;
using BivvySpot.Model.Dtos;
using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;
using Moq;
using NetTopologySuite.Geometries;

namespace BivvySpot.ApplicationTests;

public class LocationServiceTests
{
    private readonly Mock<ILocationRepository> _locRepo = new();
    private readonly Mock<ILocationSuggestionRepository> _suggRepo = new();
    private readonly Mock<IPostRepository> _postRepo = new();
    private readonly Mock<IAccountService> _accountService = new();
    private readonly Mock<IGeometryParser> _geom = new();

    private ILocationService CreateSut()
        => new LocationService(_locRepo.Object, _suggRepo.Object, _postRepo.Object, _accountService.Object, _geom.Object);

    // --- CreateAsync ---
    [Fact]
    public async Task CreateAsync_Creates_Location_When_Valid_And_Not_Duplicate()
    {
        var req = new CreateLocationDto
        {
            Name = "Ben Nevis",
            Type = LocationType.Peak,
            Latitude = 56.7969,
            Longitude = -5.0036,
            CountryCode = "gb",
            ParentId = null,
            AltNames = new List<AltNameDto> { new("Beinn Nibheis", "gd") }
        };

        _locRepo.Setup(r => r.ExistsDuplicateAsync("Ben Nevis", LocationType.Peak, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
        var point = new Point(-5.0036, 56.7969) { SRID = 4326 };
        _geom.Setup(g => g.BuildPoint(56.7969, -5.0036)).Returns(point);
        _locRepo.Setup(r => r.ExistsNearbyDuplicateAsync(LocationType.Peak, point, 100, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

        BivvySpot.Model.Entities.Location? added = null;
        _locRepo.Setup(r => r.AddAsync(It.IsAny<BivvySpot.Model.Entities.Location>(), It.IsAny<CancellationToken>()))
                .Callback<BivvySpot.Model.Entities.Location, CancellationToken>((l, _) => added = l)
                .Returns(Task.CompletedTask);

        var sut = CreateSut();
        var result = await sut.CreateAsync(req, CancellationToken.None);

        Assert.NotNull(added);
        Assert.Equal("Ben Nevis", added!.Name);
        Assert.Equal(LocationType.Peak, added.LocationType);
        _locRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(result.Id, added.Id);
    }

    [Fact]
    public async Task CreateAsync_Throws_When_Duplicate_NameOrAlias()
    {
        var req = new CreateLocationDto { Name = "Dup", Type = LocationType.Peak, Latitude = 0, Longitude = 0 };
        _locRepo.Setup(r => r.ExistsDuplicateAsync("Dup", LocationType.Peak, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        var sut = CreateSut();
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateAsync(req, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_Throws_When_Nearby_Duplicate()
    {
        var req = new CreateLocationDto { Name = "X", Type = LocationType.Peak, Latitude = 1, Longitude = 2 };
        var point = new Point(2, 1) { SRID = 4326 };
        _locRepo.Setup(r => r.ExistsDuplicateAsync("X", LocationType.Peak, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
        _geom.Setup(g => g.BuildPoint(1, 2)).Returns(point);
        _locRepo.Setup(r => r.ExistsNearbyDuplicateAsync(LocationType.Peak, point, 100, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        var sut = CreateSut();
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateAsync(req, CancellationToken.None));
    }

    // --- SuggestAsync ---
    [Fact]
    public async Task SuggestAsync_Throws_When_Not_Authenticated()
    {
        var sut = CreateSut();
        var req = new CreateLocationSuggestionDto { Name = "Y", LocationType = LocationType.Region, Latitude = 1, Longitude = 2 };
        var unauth = new AuthContext(null, null, null, null);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.SuggestAsync(unauth, req, CancellationToken.None));
    }

    [Fact]
    public async Task SuggestAsync_Creates_Suggestion_When_Valid()
    {
        var auth = new AuthContext("auth0", "sub", "me@example.com", "Me");
        var user = new User("me", "", "", "me@example.com");
        _accountService.Setup(a => a.GetCurrentProfileAsync(auth, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new AccountProfileResponse(user.Id, user.Username, user.Email, user.FirstName, user.LastName));

        var req = new CreateLocationSuggestionDto { Name = "Blue Region", LocationType = LocationType.Region, Latitude = 10, Longitude = 20, CountryCode = "US" };
        var point = new Point(20, 10) { SRID = 4326 };
        _geom.Setup(g => g.BuildPoint(10, 20)).Returns(point);
        _locRepo.Setup(r => r.ExistsNearbyDuplicateAsync(LocationType.Region, point, 100, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

        LocationSuggestion? added = null;
        _suggRepo.Setup(r => r.AddAsync(It.IsAny<LocationSuggestion>(), It.IsAny<CancellationToken>()))
                 .Callback<LocationSuggestion, CancellationToken>((s, _) => added = s)
                 .Returns(Task.CompletedTask);

        var sut = CreateSut();
        await sut.SuggestAsync(auth, req, CancellationToken.None);

        Assert.NotNull(added);
        Assert.Equal(user.Id, added!.SubmittedByUserId);
        Assert.Equal("Blue Region", added.Name);
        _suggRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SuggestAsync_Throws_When_Nearby_Duplicate()
    {
        var auth = new AuthContext("auth0", "sub", "me@example.com", "Me");
        _accountService.Setup(a => a.GetCurrentProfileAsync(auth, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new AccountProfileResponse(Guid.NewGuid(), "me", "me@example.com", "", ""));
        var req = new CreateLocationSuggestionDto { Name = "Blue Region", LocationType = LocationType.Region, Latitude = 10, Longitude = 20 };
        var point = new Point(20, 10) { SRID = 4326 };
        _geom.Setup(g => g.BuildPoint(10, 20)).Returns(point);
        _locRepo.Setup(r => r.ExistsNearbyDuplicateAsync(LocationType.Region, point, 100, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        var sut = CreateSut();
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.SuggestAsync(auth, req, CancellationToken.None));
    }

    // --- ApproveSuggestionAsync ---
    [Fact]
    public async Task ApproveSuggestionAsync_Creates_Location_And_Marks_Suggestion_Approved()
    {
        var sugg = new LocationSuggestion(Guid.NewGuid(), "Peak X", LocationType.Peak, new Point(3, 4), "GB", null, null);
        LocationSuggestion? stored = sugg;
        _suggRepo.Setup(r => r.GetAsync(sugg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(() => stored);

        BivvySpot.Model.Entities.Location? added = null;
        _locRepo.Setup(r => r.AddAsync(It.IsAny<BivvySpot.Model.Entities.Location>(), It.IsAny<CancellationToken>()))
                .Callback<BivvySpot.Model.Entities.Location, CancellationToken>((l, _) => added = l)
                .Returns(Task.CompletedTask);

        var sut = CreateSut();
        var loc = await sut.ApproveSuggestionAsync(sugg.Id, CancellationToken.None);

        Assert.NotNull(added);
        Assert.Equal("Peak X", added!.Name);
        Assert.Equal(LocationType.Peak, added.LocationType);
        Assert.Equal(SuggestionStatus.Approved, sugg.Status);
        Assert.Equal(added.Id, sugg.ApprovedLocationId);
        _locRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApproveSuggestionAsync_Throws_When_Suggestion_Not_Open()
    {
        var sugg = new LocationSuggestion(Guid.NewGuid(), "Already", LocationType.Region, new Point(1, 2), null, null, null);
        sugg.MarkApproved(Guid.NewGuid());
        _suggRepo.Setup(r => r.GetAsync(sugg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sugg);
        var sut = CreateSut();
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ApproveSuggestionAsync(sugg.Id, CancellationToken.None));
    }

    [Fact]
    public async Task ApproveSuggestionAsync_Throws_When_Suggestion_Not_Found()
    {
        _suggRepo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((LocationSuggestion?)null);
        var sut = CreateSut();
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.ApproveSuggestionAsync(Guid.NewGuid(), CancellationToken.None));
    }

    // --- SearchAsync ---
    [Fact]
    public async Task SearchAsync_Returns_Empty_When_Query_Blank()
    {
        var sut = CreateSut();
        var res = await sut.SearchAsync("   ", null, 10, CancellationToken.None);
        Assert.Empty(res);
        _locRepo.Verify(r => r.SearchByNameOrAliasAsync(It.IsAny<string>(), It.IsAny<LocationType?>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_Trims_And_Caps_Limit()
    {
        _locRepo.Setup(r => r.SearchByNameOrAliasAsync("region", LocationType.Region, 50, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<BivvySpot.Model.Entities.Location> { new("Blue Region", LocationType.Region, null, null, null, null) });
        var sut = CreateSut();
        var res = await sut.SearchAsync("  region  ", LocationType.Region, 500, CancellationToken.None);
        Assert.Single(res);
    }

    // --- ReplacePostLocationsAsync ---
    [Fact]
    public async Task ReplacePostLocationsAsync_Clears_All_When_Empty_Input()
    {
        var postId = Guid.NewGuid();
        _postRepo.Setup(r => r.GetPostLocationIdsAsync(postId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new HashSet<Guid> { Guid.NewGuid(), Guid.NewGuid() });
        var removed = 0;
        _postRepo.Setup(r => r.RemoveLocationFromPostAsync(postId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                 .Callback(() => removed++)
                 .Returns(Task.CompletedTask);
        var sut = CreateSut();
        await sut.ReplacePostLocationsAsync(postId, Array.Empty<Guid>(), CancellationToken.None);
        Assert.True(removed >= 2);
        _postRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReplacePostLocationsAsync_Throws_When_Invalid_Locations()
    {
        var postId = Guid.NewGuid();
        var list = new[] { Guid.NewGuid(), Guid.NewGuid() };
        _locRepo.Setup(r => r.AreActiveAsync(list, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var sut = CreateSut();
        await Assert.ThrowsAsync<ArgumentException>(() => sut.ReplacePostLocationsAsync(postId, list, CancellationToken.None));
    }

    [Fact]
    public async Task ReplacePostLocationsAsync_Adds_Removes_And_Sets_Order()
    {
        var postId = Guid.NewGuid();
        var a = Guid.NewGuid(); var b = Guid.NewGuid(); var c = Guid.NewGuid();
        var desired = new[] { a, c, b }; // order matters
        _locRepo.Setup(r => r.AreActiveAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _postRepo.Setup(r => r.GetPostLocationIdsAsync(postId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new HashSet<Guid> { a, b });

        var added = new List<(Guid id, int order)>();
        _postRepo.Setup(r => r.AddLocationToPostAsync(postId, It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                 .Callback<Guid, Guid, int, CancellationToken>((_, id, order, _) => added.Add((id, order)))
                 .Returns(Task.CompletedTask);

        var removed = new List<Guid>();
        _postRepo.Setup(r => r.RemoveLocationFromPostAsync(postId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                 .Callback<Guid, Guid, CancellationToken>((_, id, _) => removed.Add(id))
                 .Returns(Task.CompletedTask);

        var orderSet = new List<(Guid id, int order)>();
        _postRepo.Setup(r => r.SetLocationOrderAsync(postId, It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                 .Callback<Guid, Guid, int, CancellationToken>((_, id, order, _) => orderSet.Add((id, order)))
                 .Returns(Task.CompletedTask);

        var sut = CreateSut();
        await sut.ReplacePostLocationsAsync(postId, desired, CancellationToken.None);

        // c should be added at index 1; b retained but order changed from current -> index 2
        Assert.Contains(added, t => t.id == c && t.order == 1);
        Assert.DoesNotContain(b, removed);
        Assert.Contains(orderSet, t => t.id == b && t.order == 2);
        _postRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
