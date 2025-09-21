namespace BivvySpot.Data;

public class DictionaryEntity<T> where T : Enum
{
    public T Id { get; set; }
    public string Name { get; set; }

    public static DictionaryEntity<T> From(T value)
    {
        return new DictionaryEntity<T>
        {
            Id = value,
            Name = value.ToString()
        };
    }
}