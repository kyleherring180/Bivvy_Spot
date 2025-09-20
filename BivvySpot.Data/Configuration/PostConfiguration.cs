using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BivvySpot.Data.Configuration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.RouteName).HasMaxLength(150);
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasOne(x => x.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(x => x.UserId);

        // Common list query index
        builder.HasIndex(x => new { x.Status, x.CreatedDate });

        // Non-negative checks
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Post_ElevationGain_NonNegative", "\"ElevationGain\" >= 0");
            t.HasCheckConstraint("CK_Post_Duration_NonNegative", "\"Duration\" >= 0");
            t.HasCheckConstraint("CK_Post_LikeCount_NonNegative", "\"LikeCount\" >= 0");
            t.HasCheckConstraint("CK_Post_SaveCount_NonNegative", "\"SaveCount\" >= 0");
        });
    }
}