namespace BivvySpot.Model.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    
    private Tag() { /* private constructor for EF */}
}