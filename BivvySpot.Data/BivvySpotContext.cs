using BivvySpot.Data.Configuration;
using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace BivvySpot.Data;

public class BivvySpotContext : DbContext
{
    public BivvySpotContext(DbContextOptions<BivvySpotContext> options) : base(options)
    {
    }
    
    public BivvySpotContext(string connectionString) : base(new DbContextOptionsBuilder<BivvySpotContext>().UseSqlServer(connectionString).Options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostPhoto> PostPhotos { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<Interaction> Interactions { get; set; }
    public DbSet<GpxTrack> GpxTracks { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<PostLocation> PostLocations { get; set; }
    public DbSet<PostComment> PostComments { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<LocationAltName> LocationAltNames { get; set; }
    public DbSet<LocationSuggestion> LocationSuggestions { get; set; }
    public DbSet<Difficulty> Difficulties { get; set; }
    public DbSet<PostDifficulty> PostDifficulties { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        ApplyGeneralSettings(modelBuilder);

        modelBuilder.ConfigureDictionaryTable<ActivityType>();
        modelBuilder.ConfigureDictionaryTable<GpxStatus>();
        modelBuilder.ConfigureDictionaryTable<InteractionType>();
        modelBuilder.ConfigureDictionaryTable<LocationType>();
        modelBuilder.ConfigureDictionaryTable<PostStatus>();
        modelBuilder.ConfigureDictionaryTable<ReportStatus>();
        modelBuilder.ConfigureDictionaryTable<Season>();
        modelBuilder.ConfigureDictionaryTable<SuggestionStatus>();
    }
    
    private static void ApplyGeneralSettings(ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}