namespace BivvySpot.Model.Entities;

public class User : BaseEntity
{
    public Guid Id { get; init; }
    public string Username { get; private set; } = null!;
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; } = null!;
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
}