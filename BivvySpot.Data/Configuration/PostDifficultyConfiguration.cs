using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BivvySpot.Data.Configuration;

public class PostDifficultyConfiguration : IEntityTypeConfiguration<PostDifficulty>
{
    public void Configure(EntityTypeBuilder<PostDifficulty> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne(x => x.Post)
            .WithOne(p => p.PostDifficulty)
            .HasForeignKey<PostDifficulty>(x => x.PostId);

        builder.HasOne(x => x.Difficulty)
            .WithMany(d => d.PostDifficulties)
            .HasForeignKey(x => x.DifficultyId);

        // Enforce only one difficulty per post at the database level
        builder.HasIndex(x => x.PostId).IsUnique();
    }
}