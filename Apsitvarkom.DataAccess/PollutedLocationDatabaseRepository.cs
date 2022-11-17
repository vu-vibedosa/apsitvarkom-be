﻿using System.Linq.Expressions;
using Apsitvarkom.Models;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Class for <see cref="PollutedLocation" /> data handling using database.
/// </summary>
public class PollutedLocationDatabaseRepository : IPollutedLocationRepository
{
    private readonly IPollutedLocationContext _context;

    /// <summary>Constructor for the reader.</summary>
    /// <param name="context">Database context for <see cref="PollutedLocation"/>.</param>
    public PollutedLocationDatabaseRepository(IPollutedLocationContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PollutedLocation>> GetAllAsync()
    {
        return await _context.PollutedLocations.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PollutedLocation>> GetAllAsync(Coordinates inRelationTo)
    {
        return from pollutedLocation in await _context.PollutedLocations.ToListAsync()
            orderby pollutedLocation.Location.Coordinates.DistanceTo(inRelationTo)
            select pollutedLocation;
    }

    /// <inheritdoc />
    public Task<PollutedLocation?> GetByPropertyAsync(Expression<Func<PollutedLocation, bool>> propertyCondition)
    {
        return _context.PollutedLocations.FirstOrDefaultAsync(propertyCondition);
    }

    /// <inheritdoc />
    public async Task<PollutedLocation> InsertAsync(PollutedLocation modelToInsert)
    {
        _context.PollutedLocations.Add(modelToInsert);
        await _context.Instance.SaveChangesAsync();

        return modelToInsert;
    }
}
