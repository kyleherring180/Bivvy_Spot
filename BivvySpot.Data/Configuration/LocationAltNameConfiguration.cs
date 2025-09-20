using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BivvySpot.Data.Configuration;

public class LocationAltNameConfiguration : IEntityTypeConfiguration<LocationAltName>
{
    public void Configure(EntityTypeBuilder<LocationAltName> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Language).HasMaxLength(50);

        builder.HasOne(x => x.Location)
            .WithMany(l => l.AltNames)
            .HasForeignKey(x => x.LocationId);

        builder.HasIndex(x => new { x.LocationId, x.Name, x.Language }).IsUnique();
    }
}