using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.IntegrationTests.DataAccess;

public class DatabaseTests
{
    private DbContextOptions<PollutedLocationContext> _options = null!;

    [SetUp]
    public async Task SetUp()
    {
        _options = new DbContextOptionsBuilder<PollutedLocationContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        // Insert seed data into the database using one instance of the context
        await using var context = new PollutedLocationContext(_options);
        await DbInitializer.InitializePollutedLocations(context);
    }

    [Test]
    public async Task GetAllTest()
    {
        var instanceGuids = DbInitializer.FakePollutedLocations.Value.Select(location => location.Id).ToArray();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);
        var response = (await dbRepository.GetAllAsync()).ToArray();

        Assert.That(response.Length, Is.EqualTo(instanceGuids.Length));
        Assert.That(response.Select(x => x.Id), Is.EqualTo(instanceGuids));
    }

    [Test]
    public async Task GetAllSortedTest()
    {
        var instanceGuids = DbInitializer.FakePollutedLocations.Value.Select(location => location.Id).ToArray();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);
        var coordinates = new Coordinates { Latitude = 12.123, Longitude = -12.123 };
        var response = (await dbRepository.GetAllAsync(coordinates)).ToArray();

        Assert.That(response.Length, Is.EqualTo(instanceGuids.Length));
        Assert.That(response.Select(x => x.Id), Is.Not.EqualTo(instanceGuids));
        Assert.That(response.Select(x => x.Id), Is.EquivalentTo(instanceGuids));
    }

    [Test]
    public async Task GetByPropertyTest()
    {
        var dbRow = DbInitializer.FakePollutedLocations.Value.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        var response = await dbRepository.GetByPropertyAsync(x => x.Id == dbRow.Id);

        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(dbRow.Id));
            Assert.That(response.Location.Coordinates.Latitude, Is.EqualTo(dbRow.Location.Coordinates.Latitude));
            Assert.That(response.Location.Coordinates.Longitude, Is.EqualTo(dbRow.Location.Coordinates.Longitude));
            Assert.That(response.Radius, Is.EqualTo(dbRow.Radius));
            Assert.That(response.Severity, Is.EqualTo(dbRow.Severity));
            Assert.That(response.Spotted, Is.EqualTo(dbRow.Spotted));
            Assert.That(response.Progress, Is.EqualTo(dbRow.Progress));
            Assert.That(response.Notes, Is.EqualTo(dbRow.Notes));
        });
    }

    [Test]
    public async Task InsertAsync_SamePrimaryKeys_Throws()
    {
        // Try to insert the same row that was inserted in [SetUp]
        var dbRow = DbInitializer.FakePollutedLocations.Value.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        Assert.ThrowsAsync<ArgumentException>(() => dbRepository.InsertAsync(dbRow));
    }

    [Test]
    public async Task UpdateAsync_InstanceDoesNotExist_Throws()
    {
        // gwt existing record from database
        var dbRow = DbInitializer.FakePollutedLocations.Value.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        var locationToUpdate = new PollutedLocation
        {
            Id = Guid.NewGuid(),
            Notes = dbRow.Notes,
            Location = dbRow.Location,
            Progress = dbRow.Progress,
            Radius = dbRow.Radius,
            Severity = dbRow.Severity,
            Spotted = dbRow.Spotted,
        };

        Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => dbRepository.UpdateAsync(locationToUpdate));
    }

    [Test]
    public async Task UpdateAsync_InstanceExists_SuccessfullyUpdated()
    {
        // gwt existing record from database
        var dbRow = DbInitializer.FakePollutedLocations.Value.TakeLast(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        var notes = "test notes";
        var progress = 15;

        dbRow.Notes = notes;
        dbRow.Progress = progress;

        Assert.DoesNotThrowAsync(() => dbRepository.UpdateAsync(dbRow));
        var response = dbRepository.UpdateAsync(dbRow);

        var updatedObject = await dbRepository.GetByPropertyAsync(x => x.Id == dbRow.Id);

        Assert.That(updatedObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            //if null values were not changed
            Assert.That(updatedObject.Radius, Is.EqualTo(dbRow.Radius));
            Assert.That(updatedObject.Severity, Is.EqualTo(dbRow.Severity));
            //checking if values that are not null were changed
            Assert.That(updatedObject.Progress, Is.EqualTo(progress));
            Assert.That(updatedObject.Notes, Is.EqualTo(notes));
            //checking if other values remain the same
            Assert.That(updatedObject.Spotted, Is.EqualTo(dbRow.Spotted));
        });
    }

    [Test]
    public async Task DeleteAsync_InstanceDoesNotExist_Throws()
    {
        // Try to delete a newly created instance
        var instanceToDelete = new PollutedLocation();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => dbRepository.DeleteAsync(instanceToDelete));
    }

    [Test]
    public async Task DeleteAsync_InstanceExists_SuccessfullyDeleted()
    {
        // Try to delete one of the values that was inserted in [SetUp]
        var dbRow = DbInitializer.FakePollutedLocations.Value.TakeLast(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        Assert.DoesNotThrowAsync(() => dbRepository.DeleteAsync(dbRow));

        Assert.That((await dbRepository.GetAllAsync()).Select(x => x.Id), Does.Not.Contain(dbRow.Id));
    }
}