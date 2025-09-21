namespace BivvySpot.Model.Entities;

public class User : BaseEntity
{
    public Guid Id { get; init; }
    public string Username { get; private set; } = null!;
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; } = null!;
    public string? AuthProvider { get; private set; }     // e.g., "auth0"
    public string? AuthSubject { get; private set; }      // e.g., "auth0|abc123"
    
    public DateTimeOffset UpdatedDate { get; private set; }
    public DateTimeOffset? DeletedDate { get; private set; }

    public IReadOnlyCollection<Post> Posts { get; set; } = new List<Post>();
    public IReadOnlyCollection<PostComment> Comments { get; set; } = new List<PostComment>();
    public IReadOnlyCollection<Interaction> Interactions { get; set; } = new List<Interaction>();
    
    private User() { /*Private constructor for EF */ }
    
    public User(string username, string firstName, string lastName, string email)
    {
        Id = Guid.NewGuid();
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        SetCreatedDate();
        UpdatedDate = CreatedDate;
    }
    
    public void UpdateProfile(string? username = null, string? firstName = null, string? lastName = null)
    {
        if (!string.IsNullOrWhiteSpace(username)) Username = username.Trim();
        if (firstName is not null) FirstName = firstName.Trim();
        if (lastName  is not null) LastName  = lastName.Trim();
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        Email = email.Trim().ToLowerInvariant();
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    
    public void LinkIdentity(string provider, string subject)
    {
        AuthProvider = provider;
        AuthSubject = subject;
        UpdatedDate = DateTimeOffset.UtcNow;
    }
}