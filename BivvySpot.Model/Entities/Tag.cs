using System.Collections.ObjectModel;

namespace BivvySpot.Model.Entities;

public class Tag
{
    private readonly List<PostTag> _postTags = new();
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public ReadOnlyCollection<PostTag> PostTags => _postTags.AsReadOnly();
    
    private Tag() { /* private constructor for EF */}
}