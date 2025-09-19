namespace BivvySpot.Model.Entities;

public class BaseEntity
{
    public DateTimeOffset CreatedDate { get; private set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    
    internal void SetCreatedDate()
    {
        CreatedDate = DateTimeOffset.UtcNow;
    }
}