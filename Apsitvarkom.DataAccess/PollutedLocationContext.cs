using Apsitvarkom.Models;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.DataAccess;

public class PollutedLocationContext : DbContext
{
    public DbSet<PollutedLocation> PollutedLocations { get; set; } = null!;

    public PollutedLocationContext(DbContextOptions<PollutedLocationContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Coordinates must be specified as an owned entity type
        // Reference: https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities
        modelBuilder.Entity<PollutedLocation>().OwnsOne(l => l.Coordinates);
    }
}