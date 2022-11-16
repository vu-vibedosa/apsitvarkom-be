using System.Linq.Expressions;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Interface for <see cref="T" /> class data handling.
/// </summary>
public interface IRepository<T> where T: class
{
    /// <summary>Gets all <see cref="T" /> records from the data source.</summary>
    /// <returns><see cref="Enumerable" /> of <see cref="T" /> instances.</returns>
    public Task<IEnumerable<T>> GetAllAsync();

    /// <summary>Gets a single <see cref="T" /> record from the data source by condition.</summary>
    /// <param name="propertyCondition">Condition for the requested record acquisition.</param>
    /// <returns>Null if the instance was not found. Otherwise the acquired <see cref="T" /> instance.</returns>
    public Task<T?> GetByPropertyAsync(Expression<Func<T, bool>> propertyCondition);
}