using System.Linq.Expressions;
using Apsitvarkom.Models;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Class for <see cref="CleaningEvent" /> data handling using database.
/// </summary>
public class CleaningEventDatabaseRepository : IRepository<CleaningEvent>
{
    private readonly IPollutedLocationContext _context;

    /// <summary>Constructor for the reader.</summary>
    /// <param name="context">Database context for <see cref="CleaningEvent" />.</param>
    public CleaningEventDatabaseRepository(IPollutedLocationContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CleaningEvent>> GetAllAsync()
    {
        return await _context.CleaningEvents.ToListAsync();
    }

    /// <inheritdoc />
    public Task<CleaningEvent?> GetByPropertyAsync(Expression<Func<CleaningEvent, bool>> propertyCondition)
    {
        return _context.CleaningEvents.FirstOrDefaultAsync(propertyCondition);
    }

    /// <inheritdoc />
    public Task<bool> ExistsByPropertyAsync(Expression<Func<CleaningEvent, bool>> propertyCondition)
    {
        return _context.CleaningEvents.AnyAsync(propertyCondition);
    }

    /// <inheritdoc />
    public async Task InsertAsync(CleaningEvent modelToInsert)
    {
        await _context.CleaningEvents.AddAsync(modelToInsert);
        await _context.Instance.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(CleaningEvent modelToUpdate)
    {
        _context.CleaningEvents.Update(modelToUpdate);
        await _context.Instance.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(CleaningEvent modelToDelete)
    {
        _context.CleaningEvents.Remove(modelToDelete);
        await _context.Instance.SaveChangesAsync();
    }

    /// <inheritdoc />
    public Task<bool> ParentExistsByPropertyAsync(Expression<Func<PollutedLocation, bool>> propertyCondition)
    {
       return _context.PollutedLocations.AnyAsync(propertyCondition);
    }
}