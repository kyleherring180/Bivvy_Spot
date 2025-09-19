using BivvySpot.Model.Enums;

namespace BivvySpot.Model.Entities;

public class Interaction : BaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public InteractionType InteractionType { get; set; }

    public User User { get; set; } = null!;
    public Post Post { get; set; } = null!;
}