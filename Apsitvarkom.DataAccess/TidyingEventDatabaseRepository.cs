using System.Linq.Expressions;
using Apsitvarkom.Models;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Class for <see cref="TidyingEvent" /> data handling using database.
/// </summary>
public class TidyingEventDatabaseRepository : IRepository<TidyingEvent>
{
    private readonly IPollutedLocationContext _context;

    /// <summary>Constructor for the reader.</summary>
    /// <param name="context">Database context for <see cref="TidyingEvent" />.</param>
    public TidyingEventDatabaseRepository(IPollutedLocationContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TidyingEvent>> GetAllAsync()
    {
        return await _context.TidyingEvents.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<TidyingEvent?> GetByPropertyAsync(Expression<Func<TidyingEvent, bool>> propertyCondition)
    {
        return await _context.TidyingEvents.FirstOrDefaultAsync(propertyCondition);
    }

    /// <inheritdoc />
    public async Task InsertAsync(TidyingEvent modelToInsert)
    {
        await _context.TidyingEvents.AddAsync(modelToInsert);
        await _context.Instance.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(TidyingEvent modelToDelete)
    {
        _context.TidyingEvents.Remove(modelToDelete);
        await _context.Instance.SaveChangesAsync();
    }
}