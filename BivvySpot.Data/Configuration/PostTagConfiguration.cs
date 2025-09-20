using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BivvySpot.Data.Configuration;

public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
{
    public void Configure(EntityTypeBuilder<PostTag> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasKey(x => new { x.PostId, x.TagId });
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne(x => x.Post)
            .WithMany(p => p.PostTags)
            .HasForeignKey(x => x.PostId);

        builder.HasOne(x => x.Tag)
            .WithMany(t => t.PostTags)
            .HasForeignKey(x => x.TagId);
    }
}