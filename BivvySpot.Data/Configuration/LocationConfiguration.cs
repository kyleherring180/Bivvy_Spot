using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BivvySpot.Data.Configuration;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
        builder.Property(x => x.CountryCode).HasMaxLength(2);
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        // SQL Server: map to 'geography' (or 'geometry' if you prefer planar)
        builder.Property(x => x.Point).HasColumnType("geography");
        builder.Property(x => x.Boundary).HasColumnType("geography");

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId);

        builder.HasIndex(x => x.ParentId);

        // Keep de-dupe uniqueness
        builder.HasIndex(x => new { x.ParentId, x.Name, x.LocationType }).IsUnique();

        builder.ToTable(t =>
        {
            // Use SQL Server brackets in check constraints
            t.HasCheckConstraint("CK_Location_Elevation_NonNegative", "[Elevation] IS NULL OR [Elevation] >= 0");
        });
    }
}