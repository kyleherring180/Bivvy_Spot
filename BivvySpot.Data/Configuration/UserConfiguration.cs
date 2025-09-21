using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BivvySpot.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Username).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();

        // Optional new identity fields (if present on your model)
        builder.Property<string?>("AuthProvider").HasMaxLength(64);
        builder.Property<string?>("AuthSubject").HasMaxLength(200);

        // RowVersion from BaseEntity
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        // Soft-delete–aware uniques for Email & Username
        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasFilter("[DeletedDate] IS NULL");

        builder.HasIndex(x => x.Username)
            .IsUnique()
            .HasFilter("[DeletedDate] IS NULL");

        // Unique identity link when both values are present
        builder.HasIndex("AuthProvider", "AuthSubject")
            .IsUnique()
            .HasFilter("[AuthProvider] IS NOT NULL AND [AuthSubject] IS NOT NULL AND [DeletedDate] IS NULL");

        // (Optional) normalize email at DB level using a computed column if you like.
        // Otherwise, keep normalizing in code (ToLowerInvariant + Trim).
    }
}