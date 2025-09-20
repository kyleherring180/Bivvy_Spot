using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BivvySpot.Data.Configuration;

public class GpxTrackConfiguration : IEntityTypeConfiguration<GpxTrack>
{
    public void Configure(EntityTypeBuilder<GpxTrack> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.StorageKey).HasMaxLength(512).IsRequired();
        builder.Property(x => x.ChecksumSha256).HasMaxLength(64).IsRequired();
        builder.Property(x => x.PreviewGeoJsonKey).HasMaxLength(512);
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        // SQL Server: use geography/geometry. 4326 is the standard WGS84 SRID for geography.
        builder.Property(x => x.Bbox).HasColumnType("geography");

        builder.HasOne(x => x.Post)
            .WithMany(p => p.GpxTracks)
            .HasForeignKey(x => x.PostId);

        builder.HasIndex(x => x.PostId);

        builder.ToTable(t =>
        {
            // SQL Server uses [square brackets] in check constraints
            t.HasCheckConstraint("CK_Gpx_SizeBytes_NonNegative", "[SizeBytes] >= 0");
            t.HasCheckConstraint("CK_Gpx_Distance_NonNegative", "[Distance] >= 0");
            t.HasCheckConstraint("CK_Gpx_AscentM_NonNegative", "[AscentM] >= 0");
        });
    }
}