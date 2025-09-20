using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BivvySpot.Data.Configuration;

public class InteractionConfiguration : IEntityTypeConfiguration<Interaction>
{
    public void Configure(EntityTypeBuilder<Interaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasOne(x => x.User)
            .WithMany(u => u.Interactions)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Post)
            .WithMany(p => p.Interactions)
            .HasForeignKey(x => x.PostId);

        // One interaction per user/post/type (like OR save)
        builder.HasIndex(x => new { x.UserId, x.PostId, x.InteractionType }).IsUnique();
    }
}