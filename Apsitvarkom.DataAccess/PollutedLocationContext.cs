using Apsitvarkom.Models;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.DataAccess;

public interface IPollutedLocationContext
{
    DbSet<PollutedLocation> PollutedLocations { get; }
    DbSet<CleaningEvent> CleaningEvents { get; }
    DbContext Instance { get; }
}

public class PollutedLocationContext : DbContext, IPollutedLocationContext
{
    public DbSet<PollutedLocation> PollutedLocations { get; private set; } = null!;
    public DbSet<CleaningEvent> CleaningEvents { get; private set; } = null!;
    public DbContext Instance => this;

    public PollutedLocationContext(DbContextOptions<PollutedLocationContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PollutedLocation>().Property(x => x.Severity).HasConversion(
            v => v.ToString(),
            v => (PollutedLocation.SeverityLevel)Enum.Parse(typeof(PollutedLocation.SeverityLevel), v)
        );
        var severityLevelsString = string.Join(", ", ((PollutedLocation.SeverityLevel[])Enum.GetValues(typeof(PollutedLocation.SeverityLevel))).Select(p => $"'{p}'"));
        modelBuilder.Entity<PollutedLocation>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_PollutedLocation_Radius", "\"Radius\" >= 1");
            t.HasCheckConstraint("CK_PollutedLocation_Progress", "\"Progress\" >= 0 and \"Progress\" <= 100");
            t.HasCheckConstraint("CK_PollutedLocation_Severity", $"\"Severity\" in ({severityLevelsString})");
        })
            .OwnsOne(l => l.Location)
            .OwnsOne(l => l.Coordinates).ToTable(t =>
            {
                t.HasCheckConstraint("CK_Coordinates_Latitude", "\"Location_Coordinates_Latitude\" >= -90 and \"Location_Coordinates_Latitude\" <= 90");
                t.HasCheckConstraint("CK_Coordinates_Longitude", "\"Location_Coordinates_Longitude\" >= -180 and \"Location_Coordinates_Longitude\" <= 180");
            });
        modelBuilder.Entity<PollutedLocation>().OwnsOne(l => l.Location).OwnsOne(l => l.Title);
    }
}