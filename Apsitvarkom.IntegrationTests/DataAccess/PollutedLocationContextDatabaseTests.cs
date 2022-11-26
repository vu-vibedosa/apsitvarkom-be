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
    public async Task PollutedLocation_ExistsByPropertyTest()
    {
        var dbRow = DbInitializer.FakePollutedLocations.Value.Skip(4).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        var response = await dbRepository.ExistsByPropertyAsync(x => x.Id == dbRow.Id);

        Assert.That(response, Is.True);
    }

    [Test]
    public async Task PollutedLocation_InsertAsync_SamePrimaryKeys_Throws()
    {
        // Try to insert the same row that was inserted in [SetUp]
        var dbRow = DbInitializer.FakePollutedLocations.Value.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        Assert.ThrowsAsync<ArgumentException>(async () => await dbRepository.InsertAsync(dbRow));
    }

    [Test]
    public async Task PollutedLocation_InsertAsync_InsertionWithEvents_PrimaryKeyUnique_SuccessfullyInsertedAndEventsCreated()
    {
        var instanceToInsert = new PollutedLocation
        {
            Id = Guid.NewGuid(),
            Notes = "Lithuania",
            Progress = 12,
            Radius = 3,
            Severity = PollutedLocation.SeverityLevel.High,
            Spotted = new DateTime(2022, 12, 23, 23, 59, 59).ToUniversalTime(),
            Location = new Location
            {
                Title = "Name",
                Coordinates = new Coordinates
                {
                    Latitude = 12.00,
                    Longitude = 13.00
                }
            },
            Events = new List<TidyingEvent>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Notes = "Harry Potter",
                    StartTime = DateTime.UtcNow.ToUniversalTime()
                }
            }
        };

        // Use a clean instance of the context to run the test
        await using var context1 = new PollutedLocationContext(_options);
        await using var context2 = new PollutedLocationContext(_options);
        var locationDbRepository = new PollutedLocationDatabaseRepository(context1);
        var eventDbRepository = new TidyingEventDatabaseRepository(context2);

        Assert.DoesNotThrowAsync(async () => await locationDbRepository.InsertAsync(instanceToInsert));

        var locations = (await locationDbRepository.GetAllAsync()).ToArray();
        Assert.That(locations.Length, Is.EqualTo(DbInitializer.FakePollutedLocations.Value.Length + 1));
        Assert.That(locations.Select(x => x.Id).Contains(instanceToInsert.Id), Is.True);

        var events = (await eventDbRepository.GetAllAsync()).ToArray();
        Assert.That(events.Length, Is.EqualTo(DbInitializer.FakeTidyingEvents.Value.Length + 1));
        Assert.That(events.Select(x => x.Id).Contains(instanceToInsert.Events.Single().Id), Is.True);
    }

    [Test]
    public async Task PollutedLocation_DeleteAsync_InstanceDoesNotExist_Throws()
    {
        // Try to delete a newly created instance
        var instanceToDelete = new PollutedLocation();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await dbRepository.DeleteAsync(instanceToDelete));
    }

    [Test]
    public async Task PollutedLocation_DeleteAsync_InstanceExists_SuccessfullyDeleted()
    {
        // Try to delete one of the values that was inserted in [SetUp]
        var dbRow = DbInitializer.FakePollutedLocations.Value.TakeLast(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        Assert.DoesNotThrowAsync(async () => await dbRepository.DeleteAsync(dbRow));

        Assert.That((await dbRepository.GetAllAsync()).Select(x => x.Id), Does.Not.Contain(dbRow.Id));
    }

    [Test]
    public async Task PollutedLocation_DeleteAsync_LocationWithEventsExists_SuccessfullyDeleted_AssociatedEventsDeletedTogether()
    {
        // Try to delete one of the values that was inserted in [SetUp]
        var dbRow = DbInitializer.FakePollutedLocations.Value.Skip(3).Take(1).Single();

        Assert.That(dbRow.Events, Is.Not.Empty);

        // Use a clean instance of the context to run the test
        await using var context1 = new PollutedLocationContext(_options);
        await using var context2 = new PollutedLocationContext(_options);
        var locationDbRepository = new PollutedLocationDatabaseRepository(context1);
        var eventDbRepository = new TidyingEventDatabaseRepository(context2);

        Assert.DoesNotThrowAsync(async () => await locationDbRepository.DeleteAsync(dbRow));

        Assert.That((await locationDbRepository.GetAllAsync()).Select(x => x.Id), Does.Not.Contain(dbRow.Id));

        foreach (var eventId in dbRow.Events.Select(x => x.Id))
        {
            Assert.That((await eventDbRepository.GetAllAsync()).Select(x => x.Id), Does.Not.Contain(eventId));

        }
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
    public async Task TidyingEvent_ExistsByPropertyTest()
    {
        var dbRow = DbInitializer.FakeTidyingEvents.Value.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new TidyingEventDatabaseRepository(context);

        var response = await dbRepository.ExistsByPropertyAsync(x => x.Id == dbRow.Id);

        Assert.That(response, Is.True);
    }

    [Test]
    public async Task TidyingEvent_InsertAsync_SamePrimaryKeys_Throws()
    {
        // Try to insert the same row that was inserted in [SetUp]
        var dbRow = DbInitializer.FakeTidyingEvents.Value.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new TidyingEventDatabaseRepository(context);

        Assert.ThrowsAsync<ArgumentException>(async () => await dbRepository.InsertAsync(dbRow));
    }

    [Test]
    public async Task TidyingEvent_InsertAsync_PrimaryKeyUnique_SuccessfullyInserted()
    {
        var instanceToInsert = new TidyingEvent
        {
            Id = Guid.NewGuid(),
            PollutedLocationId = Guid.NewGuid(),
            Notes = "Harry Potter",
            StartTime = DateTime.UtcNow.ToUniversalTime()
        };

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var eventDbRepository = new TidyingEventDatabaseRepository(context);

        Assert.DoesNotThrowAsync(async () => await eventDbRepository.InsertAsync(instanceToInsert));

        var events = (await eventDbRepository.GetAllAsync()).ToArray();
        Assert.That(events.Length, Is.EqualTo(DbInitializer.FakeTidyingEvents.Value.Length + 1));
        Assert.That(events.Select(x => x.Id).Contains(instanceToInsert.Id), Is.True);
    }

    [Test]
    public async Task TidyingEvent_DeleteAsync_InstanceDoesNotExist_Throws()
    {
        // Try to delete a newly created instance
        var instanceToDelete = new TidyingEvent();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new TidyingEventDatabaseRepository(context);

        Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await dbRepository.DeleteAsync(instanceToDelete));
    }

    [Test]
    public async Task TidyingEvent_DeleteAsync_InstanceExists_SuccessfullyDeleted_PollutedLocationDoesNotInclude()
    {
        // Try to delete one of the values that was inserted in [SetUp]
        var dbRow = DbInitializer.FakeTidyingEvents.Value.Skip(3).Take(1).First();

        // Use a clean instance of the context to run the test
        await using var context1 = new PollutedLocationContext(_options);
        await using var context2 = new PollutedLocationContext(_options);
        var locationDbRepository = new PollutedLocationDatabaseRepository(context2); 
        var eventDbRepository = new TidyingEventDatabaseRepository(context1);

        Assert.DoesNotThrowAsync(async () => await eventDbRepository.DeleteAsync(dbRow));

        Assert.That((await eventDbRepository.GetAllAsync()).Select(x => x.Id), Does.Not.Contain(dbRow.Id));

        var pollutedLocation = await locationDbRepository.GetByPropertyAsync(x => x.Id == dbRow.PollutedLocationId);

        Assert.That(pollutedLocation, Is.Not.Null);
        Assert.That(pollutedLocation.Events.Select(x => x.Id), Does.Not.Contain(dbRow.Id));
    }
    #endregion
}