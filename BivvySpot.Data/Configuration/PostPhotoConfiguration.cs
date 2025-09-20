using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BivvySpot.Data.Configuration;

public class PostPhotoConfiguration : IEntityTypeConfiguration<PostPhoto>
{
    public void Configure(EntityTypeBuilder<PostPhoto> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.StorageKey).HasMaxLength(512).IsRequired();
        builder.Property(x => x.AltText).HasMaxLength(300);
        builder.Property(x => x.Caption).HasMaxLength(1000);
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasOne(x => x.Post)
            .WithMany(p => p.Photos)
            .HasForeignKey(x => x.PostId);

        builder.HasIndex(x => new { x.PostId, x.SortOrder }).IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_PostPhoto_Width_NonNegative", "\"Width\" >= 0");
            t.HasCheckConstraint("CK_PostPhoto_Height_NonNegative", "\"Height\" >= 0");
            t.HasCheckConstraint("CK_PostPhoto_SortOrder_NonNegative", "\"SortOrder\" >= 0");
        });
    }
}