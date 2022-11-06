using Apsitvarkom.Models;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.DataAccess;

public class PollutedLocationContext : DbContext
{
    public DbSet<PollutedLocation> PollutedLocations { get; set; } = null!;

    public PollutedLocationContext(DbContextOptions<PollutedLocationContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PollutedLocation>().OwnsOne(l => l.Coordinates);
    }
}
