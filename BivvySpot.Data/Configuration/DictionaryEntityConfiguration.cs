using Microsoft.EntityFrameworkCore;

namespace BivvySpot.Data.Configuration;

public static class DictionaryEntityConfiguration
{
    private const string DictionaryTablePrefix = "Dictionary_";
    private const string DataTypeForEnum = "tinyint";
    private const int NameLength = 128;

    public static ModelBuilder ConfigureDictionaryTable<T>(this ModelBuilder builder) where T : Enum
    {
        builder.Entity<DictionaryEntity<T>>(e => e.ToTable($"{DictionaryTablePrefix}{typeof(T).Name}"));
        
        var entityTypeBuilder = builder.Entity<DictionaryEntity<T>>();
        entityTypeBuilder.HasKey(x => x.Id);
        entityTypeBuilder.Property(x => x.Id).HasColumnType(DataTypeForEnum).ValueGeneratedNever();
        entityTypeBuilder.Property(x => x.Name).HasMaxLength(NameLength);

        entityTypeBuilder.HasData(
            Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(DictionaryEntity<T>.From));

        return builder;
    }
}