using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BivvySpot.Data.Configuration;

public class LocationSuggestionConfiguration : IEntityTypeConfiguration<LocationSuggestion>
{
    public void Configure(EntityTypeBuilder<LocationSuggestion> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}