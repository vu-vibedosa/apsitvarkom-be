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

    /// <summary>Checks if a single <see cref="T" /> record exists in the data source by condition.</summary>
    /// <param name="propertyCondition">Condition for the requested record check.</param>
    /// <returns>False if the instance was not found, true otherwise.</returns>
    public Task<bool> ExistsByPropertyAsync(Expression<Func<T, bool>> propertyCondition);

    /// <summary>Inserts a single <see cref="T" /> record to the data source.</summary>
    /// <param name="modelToInsert">Model to be inserted into the repository.</param>
    /// <returns>The <see cref="Task"/> to insert the model.</returns>
    public Task InsertAsync(T modelToInsert);

    /// <summary>Deletes a single <see cref="T" /> record from the data source.</summary>
    /// <param name="modelToDelete">Model to be deleted from the repository.</param>
    /// <returns>The <see cref="Task"/> to delete the model.</returns>
    public Task DeleteAsync(T modelToDelete);
}