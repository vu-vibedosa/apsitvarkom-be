namespace Apsitvarkom.DataAccess;

/// <summary>
/// Interface for <see cref="T" /> class data handling.
/// </summary>
public interface IEntityDTORepository<T> where T: class
{
    /// <summary>Gets all <see cref="T" /> records from the data source.</summary>
    /// <returns><see cref="Enumerable" /> of <see cref="T" /> instances.</returns>
    public Task<IEnumerable<T>> GetAllAsync();

    /// <summary>Gets a single <see cref="T" /> record from the data source by id.</summary>
    /// <param name="id">Identifier of the requested record.</param>
    /// <returns>Null if the instance was not found. Otherwise a <see cref="T" /> instance with matching identifier.</returns>
    public Task<T?> GetByIdAsync(string id);
}