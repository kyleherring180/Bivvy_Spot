using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BivvySpot.Data.Configuration;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Reason).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ModeratorNote).HasMaxLength(1000);
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasOne(x => x.Post)
            .WithMany(p => p.Reports)
            .HasForeignKey(x => x.PostId);

        builder.HasOne(x => x.Reporter)
            .WithMany() // not materializing a Reports collection on User
            .HasForeignKey(x => x.ReporterId);

        builder.HasOne(x => x.Resolver)
            .WithMany()
            .HasForeignKey(x => x.ResolvedBy);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.PostId);
        builder.HasIndex(x => x.ReporterId);
    }
}