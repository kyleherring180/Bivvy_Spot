using BivvySpot.Model.Entities;

namespace BivvySpot.ModelTests;

public class UserTests
{
    [Fact]
    public void Ctor_Sets_Properties_And_Initial_Timestamp()
    {
        var before = DateTimeOffset.UtcNow.AddSeconds(-2);
        var u = new User("kyle", "Kyle", "Herring", "Kyle@example.com");

        Assert.NotEqual(Guid.Empty, u.Id);
        Assert.Equal("kyle", u.Username);
        Assert.Equal("Kyle", u.FirstName);
        Assert.Equal("Herring", u.LastName);
        Assert.Equal("Kyle@example.com", u.Email);

        // UpdatedDate is set to CreatedDate in ctor; should be ~now (UTC)
        Assert.True(u.UpdatedDate >= before);
        Assert.Null(u.AuthProvider);
        Assert.Null(u.AuthSubject);

        // Collections start empty
        Assert.Empty(u.Posts);
        Assert.Empty(u.Comments);
        Assert.Empty(u.Interactions);
    }

    [Fact]
    public void UpdateProfile_Updates_Selected_Fields_And_Trims()
    {
        var u = new User("kyle", "Kyle", "Herring", "k@example.com");
        var prevUpdated = u.UpdatedDate;

        u.UpdateProfile(username: "  new_name  ", firstName: "  K ", lastName: "  H ");

        Assert.Equal("new_name", u.Username);
        Assert.Equal("K", u.FirstName);
        Assert.Equal("H", u.LastName);
        Assert.True(u.UpdatedDate >= prevUpdated);
    }

    [Fact]
    public void UpdateProfile_Whitespace_Username_Is_Ignored()
    {
        var u = new User("kyle", "Kyle", "Herring", "k@example.com");
        var prevUsername = u.Username;
        var prevUpdated = u.UpdatedDate;

        u.UpdateProfile(username: "   ", firstName: null, lastName: null);

        // Username unchanged when whitespace-only provided
        Assert.Equal(prevUsername, u.Username);
        // Method still bumps UpdatedDate by design
        Assert.True(u.UpdatedDate >= prevUpdated);
    }

    [Fact]
    public void UpdateEmail_Normalizes_And_Bumps_Timestamp()
    {
        var u = new User("kyle", "Kyle", "Herring", "k@example.com");
        var prevUpdated = u.UpdatedDate;

        u.UpdateEmail("  JOHN.DOE@Example.COM  ");

        Assert.Equal("john.doe@example.com", u.Email);
        Assert.True(u.UpdatedDate >= prevUpdated);
    }

    [Fact]
    public void LinkIdentity_Sets_Provider_Subject_And_Bumps_Timestamp()
    {
        var u = new User("kyle", "Kyle", "Herring", "k@example.com");
        var prevUpdated = u.UpdatedDate;

        u.LinkIdentity("auth0", "auth0|abc123");

        Assert.Equal("auth0", u.AuthProvider);
        Assert.Equal("auth0|abc123", u.AuthSubject);
        Assert.True(u.UpdatedDate >= prevUpdated);
    }

    [Fact]
    public void UpdateProfile_Does_Not_Null_Out_Fields_When_Null_Params()
    {
        var u = new User("kyle", "Kyle", "Herring", "k@example.com");

        u.UpdateProfile(username: null, firstName: null, lastName: null);

        Assert.Equal("kyle", u.Username);
        Assert.Equal("Kyle", u.FirstName);
        Assert.Equal("Herring", u.LastName);
    }

    [Fact]
    public void UpdateProfile_Trims_But_Allows_Empty_First_Or_Last_When_Explicit()
    {
        // If you consider empty strings valid to *clear* names, test it.
        // Right now your code trims and assigns even empty string ("" -> ""), which is fine if desired.
        var u = new User("kyle", "Kyle", "Herring", "k@example.com");

        u.UpdateProfile(firstName: " ", lastName: "");

        Assert.Equal("", u.FirstName); // " ".Trim() => ""
        Assert.Equal("", u.LastName);
    }
}