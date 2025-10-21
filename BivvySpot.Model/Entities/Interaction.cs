using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Entities;

public class Interaction : BaseEntity
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid PostId { get; init; }
    public InteractionType InteractionType { get; private set; }

    public User User { get; private set; } = null!;
    public Post Post { get; private set; } = null!;
    
    private Interaction() { /* private constructor for EF */}

    public Interaction(Guid userId, Guid postId, InteractionType interactionType)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        PostId = postId;
        InteractionType = interactionType;
        SetCreatedDate();
    }
}