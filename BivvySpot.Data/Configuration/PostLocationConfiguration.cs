using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BivvySpot.Data.Configuration;

public class PostLocationConfiguration : IEntityTypeConfiguration<PostLocation>
{
    public void Configure(EntityTypeBuilder<PostLocation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne(x => x.Post)
            .WithMany(p => p.PostLocations)
            .HasForeignKey(x => x.PostId);

        builder.HasOne(x => x.Location)
            .WithMany(l => l.PostLocations)
            .HasForeignKey(x => x.LocationId);

        builder.HasIndex(x => new { x.PostId, x.LocationId }).IsUnique();
        builder.HasIndex(x => new { x.PostId, x.Order }).IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_PostLocation_Order_NonNegative", "\"Order\" >= 0");
        });
    }
}