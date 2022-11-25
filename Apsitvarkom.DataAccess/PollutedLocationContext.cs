using Apsitvarkom.Models;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.DataAccess;

public interface IPollutedLocationContext
{
    DbSet<PollutedLocation> PollutedLocations { get; }
    DbSet<TidyingEvent> TidyingEvents { get; }
    DbContext Instance { get; }
}

public class PollutedLocationContext : DbContext, IPollutedLocationContext
{
    public DbSet<PollutedLocation> PollutedLocations { get; private set; } = null!;

    public DbSet<TidyingEvent> TidyingEvents { get; private set; } = null!;

    public DbContext Instance => this;

    public PollutedLocationContext(DbContextOptions<PollutedLocationContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Location and coordinates must be specified as owned entity types
        // Reference: https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities
        modelBuilder.Entity<PollutedLocation>().OwnsOne(l => l.Location).OwnsOne(l => l.Coordinates);
    }
}