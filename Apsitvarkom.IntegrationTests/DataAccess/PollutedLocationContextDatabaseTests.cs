using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.IntegrationTests.DataAccess;

[Parallelizable(ParallelScope.None)]
public class PollutedLocationContextDatabaseTests
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

    #region PollutedLocation
    [Test]
    public async Task PollutedLocation_GetAllTest()
    {
        var objectIds = DbInitializer.FakePollutedLocations.Value.Select(location => location.Id).ToArray();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);
        var response = (await dbRepository.GetAllAsync()).ToArray();

        Assert.That(response.Length, Is.EqualTo(objectIds.Length));
        Assert.That(response.Select(x => x.Id), Is.EqualTo(objectIds));
    }

    [Test]
    public async Task PollutedLocation_GetAllSortedTest()
    {
        var objectIds = DbInitializer.FakePollutedLocations.Value.Select(location => location.Id).ToArray();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);
        var coordinates = new Coordinates { Latitude = 12.123, Longitude = -12.123 };
        var response = (await dbRepository.GetAllAsync(coordinates)).ToArray();

        Assert.That(response.Length, Is.EqualTo(objectIds.Length));
        Assert.That(response.Select(x => x.Id), Is.Not.EqualTo(objectIds));
        Assert.That(response.Select(x => x.Id), Is.EquivalentTo(objectIds));
    }

    [Test]
    public async Task PollutedLocation_GetByPropertyTest()
    {
        var dbRow = DbInitializer.FakePollutedLocations.Value.Skip(4).Take(1).Single();

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
            Assert.That(response.Events.Select(x => x.Id), Is.EqualTo(dbRow.Events.Select(x => x.Id)));
        });
    }

    [Test]
    public async Task PollutedLocation_InsertAsync_SamePrimaryKeys_Throws()
    {
        // Try to insert the same row that was inserted in [SetUp]
        var dbRow = DbInitializer.FakePollutedLocations.Value.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        Assert.ThrowsAsync<ArgumentException>(() => dbRepository.InsertAsync(dbRow));
    }

    [Test]
    public async Task PollutedLocation_DeleteAsync_InstanceDoesNotExist_Throws()
    {
        // Try to delete a newly created instance
        var instanceToDelete = new PollutedLocation();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => dbRepository.DeleteAsync(instanceToDelete));
    }

    [Test]
    public async Task PollutedLocation_DeleteAsync_InstanceExists_SuccessfullyDeleted()
    {
        // Try to delete one of the values that was inserted in [SetUp]
        var dbRow = DbInitializer.FakePollutedLocations.Value.TakeLast(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        Assert.DoesNotThrowAsync(() => dbRepository.DeleteAsync(dbRow));

        Assert.That((await dbRepository.GetAllAsync()).Select(x => x.Id), Does.Not.Contain(dbRow.Id));
    }
    #endregion

    #region TidyingEvent
    [Test]
    public async Task TidyingEvent_GetAllTest()
    {
        var objectIds = DbInitializer.FakeTidyingEvents.Value.Select(tidyingEvent => tidyingEvent.Id).ToArray();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new TidyingEventDatabaseRepository(context);
        var response = (await dbRepository.GetAllAsync()).ToArray();

        Assert.That(response.Length, Is.EqualTo(objectIds.Length));
        Assert.That(response.Select(x => x.Id), Is.EqualTo(objectIds));
    }

    [Test]
    public async Task TidyingEvent_GetByPropertyTest()
    {
        var dbRow = DbInitializer.FakeTidyingEvents.Value.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new TidyingEventDatabaseRepository(context);

        var response = await dbRepository.GetByPropertyAsync(x => x.Id == dbRow.Id);

        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(dbRow.Id));
            Assert.That(response.PollutedLocationId, Is.EqualTo(dbRow.PollutedLocationId));
            Assert.That(response.StartTime, Is.EqualTo(dbRow.StartTime));
            Assert.That(response.Notes, Is.EqualTo(dbRow.Notes));
        });
    }

    [Test]
    public async Task TidyingEvent_InsertAsync_SamePrimaryKeys_Throws()
    {
        // Try to insert the same row that was inserted in [SetUp]
        var dbRow = DbInitializer.FakeTidyingEvents.Value.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new TidyingEventDatabaseRepository(context);

        Assert.ThrowsAsync<ArgumentException>(() => dbRepository.InsertAsync(dbRow));
    }

    [Test]
    public async Task TidyingEvent_DeleteAsync_InstanceDoesNotExist_Throws()
    {
        // Try to delete a newly created instance
        var instanceToDelete = new TidyingEvent();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new TidyingEventDatabaseRepository(context);

        Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => dbRepository.DeleteAsync(instanceToDelete));
    }

    [Test]
    public async Task TidyingEvent_DeleteAsync_InstanceExists_SuccessfullyDeleted()
    {
        // Try to delete one of the values that was inserted in [SetUp]
        var dbRow = DbInitializer.FakeTidyingEvents.Value.TakeLast(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new TidyingEventDatabaseRepository(context);

        Assert.DoesNotThrowAsync(() => dbRepository.DeleteAsync(dbRow));

        Assert.That((await dbRepository.GetAllAsync()).Select(x => x.Id), Does.Not.Contain(dbRow.Id));
    }
    #endregion
}