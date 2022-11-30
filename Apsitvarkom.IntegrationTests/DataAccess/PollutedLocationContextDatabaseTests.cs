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
    public async Task PollutedLocation_GetAllAsyncTest()
    {
        var objectIds = DbInitializer.FakePollutedLocations.Select(location => location.Id).ToArray();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);
        var response = (await dbRepository.GetAllAsync()).ToArray();

        Assert.That(response.Length, Is.EqualTo(objectIds.Length));
        Assert.That(response.Select(x => x.Id), Is.EqualTo(objectIds));
    }

    [Test]
    public async Task PollutedLocation_GetAllAsyncSortedTest()
    {
        var objectIds = DbInitializer.FakePollutedLocations.Select(location => location.Id).ToArray();

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
    public async Task PollutedLocation_GetByPropertyAsyncTest()
    {
        var dbRow = DbInitializer.FakePollutedLocations.Skip(4).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        var response = await dbRepository.GetByPropertyAsync(x => x.Id == dbRow.Id);

        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(dbRow.Id));
            Assert.That(response.Location.Titles.Select(x => (x.Code, Title: x.Name)), Is.EqualTo(dbRow.Location.Titles.Select(x => (x.Code, Title: x.Name))));
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
    public async Task PollutedLocation_ExistsByPropertyAsyncTest()
    {
        var dbRow = DbInitializer.FakePollutedLocations.Skip(4).Take(1).Single();

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
        var dbRow = DbInitializer.FakePollutedLocations.Skip(3).Take(1).Single();

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
                Titles = 
                {
                    new() { Code = LocationTitle.LocationCode.en, Name = "text sample" },
                    new() { Code = LocationTitle.LocationCode.lt, Name = "teksto pavyzdys" },
                },
                Coordinates = new Coordinates
                {
                    Latitude = 12.00,
                    Longitude = 13.00
                }
            },
            Events = new List<CleaningEvent>
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
        var eventDbRepository = new CleaningEventDatabaseRepository(context2);

        Assert.DoesNotThrowAsync(async () => await locationDbRepository.InsertAsync(instanceToInsert));

        var locations = (await locationDbRepository.GetAllAsync()).ToArray();
        Assert.That(locations.Length, Is.EqualTo(DbInitializer.FakePollutedLocations.Length + 1));
        Assert.That(locations.Select(x => x.Id).Contains(instanceToInsert.Id), Is.True);

        var events = (await eventDbRepository.GetAllAsync()).ToArray();
        Assert.That(events.Length, Is.EqualTo(DbInitializer.FakeCleaningEvents.Length + 1));
        Assert.That(events.Select(x => x.Id).Contains(instanceToInsert.Events.Single().Id), Is.True);
    }

    [Test]
    public async Task PollutedLocation_UpdateAsync_InstanceDoesNotExist_Throws()
    {
        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        var locationToUpdate = new PollutedLocation
        {
            Id = Guid.NewGuid()
        };

        Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await dbRepository.UpdateAsync(locationToUpdate));
    }

    [Test]
    public async Task PollutedLocation_UpdateAsync_InstanceExists_SuccessfullyUpdated()
    {
        // Get an existing record from the database
        var dbRow = DbInitializer.FakePollutedLocations.TakeLast(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new PollutedLocationDatabaseRepository(context);

        dbRow.Notes = "test notes";
        dbRow.Progress = 15;

        Assert.DoesNotThrowAsync(async () => await dbRepository.UpdateAsync(dbRow));

        var updatedObject = await dbRepository.GetByPropertyAsync(x => x.Id == dbRow.Id);

        Assert.That(updatedObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(updatedObject.Id, Is.EqualTo(dbRow.Id));
            Assert.That(updatedObject.Location.Titles, Is.EqualTo(dbRow.Location.Titles));
            Assert.That(updatedObject.Location.Coordinates.Latitude, Is.EqualTo(dbRow.Location.Coordinates.Latitude));
            Assert.That(updatedObject.Location.Coordinates.Longitude, Is.EqualTo(dbRow.Location.Coordinates.Longitude));
            Assert.That(updatedObject.Radius, Is.EqualTo(dbRow.Radius));
            Assert.That(updatedObject.Severity, Is.EqualTo(dbRow.Severity));
            Assert.That(updatedObject.Spotted, Is.EqualTo(dbRow.Spotted));
            Assert.That(updatedObject.Progress, Is.EqualTo(dbRow.Progress));
            Assert.That(updatedObject.Notes, Is.EqualTo(dbRow.Notes));
        });
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
        var dbRow = DbInitializer.FakePollutedLocations.TakeLast(1).Single();

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
        var dbRow = DbInitializer.FakePollutedLocations.Skip(3).Take(1).Single();

        Assert.That(dbRow.Events, Is.Not.Empty);

        // Use a clean instance of the context to run the test
        await using var context1 = new PollutedLocationContext(_options);
        await using var context2 = new PollutedLocationContext(_options);
        var locationDbRepository = new PollutedLocationDatabaseRepository(context1);
        var eventDbRepository = new CleaningEventDatabaseRepository(context2);

        Assert.DoesNotThrowAsync(async () => await locationDbRepository.DeleteAsync(dbRow));

        Assert.That((await locationDbRepository.GetAllAsync()).Select(x => x.Id), Does.Not.Contain(dbRow.Id));

        foreach (var eventId in dbRow.Events.Select(x => x.Id))
        {
            Assert.That((await eventDbRepository.GetAllAsync()).Select(x => x.Id), Does.Not.Contain(eventId));

        }
    }
    #endregion

    #region CleaningEvent
    [Test]
    public async Task CleaningEvent_GetAllAsyncTest()
    {
        var objectIds = DbInitializer.FakeCleaningEvents.Select(cleaningEvent => cleaningEvent.Id).ToArray();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new CleaningEventDatabaseRepository(context);
        var response = (await dbRepository.GetAllAsync()).ToArray();

        Assert.That(response.Length, Is.EqualTo(objectIds.Length));
        Assert.That(response.Select(x => x.Id), Is.EqualTo(objectIds));
    }

    [Test]
    public async Task CleaningEvent_GetByPropertyAsyncTest()
    {
        var dbRow = DbInitializer.FakeCleaningEvents.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new CleaningEventDatabaseRepository(context);

        var response = await dbRepository.GetByPropertyAsync(x => x.Id == dbRow.Id);

        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(dbRow.Id));
            Assert.That(response.StartTime, Is.EqualTo(dbRow.StartTime));
            Assert.That(response.Notes, Is.EqualTo(dbRow.Notes));
        });
    }

    [Test]
    public async Task CleaningEvent_ExistsByPropertyAsyncTest()
    {
        var dbRow = DbInitializer.FakeCleaningEvents.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new CleaningEventDatabaseRepository(context);

        var response = await dbRepository.ExistsByPropertyAsync(x => x.Id == dbRow.Id);

        Assert.That(response, Is.True);
    }

    [Test]
    public async Task CleaningEvent_InsertAsync_SamePrimaryKeys_Throws()
    {
        // Try to insert the same row that was inserted in [SetUp]
        var dbRow = DbInitializer.FakeCleaningEvents.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new CleaningEventDatabaseRepository(context);

        Assert.ThrowsAsync<ArgumentException>(async () => await dbRepository.InsertAsync(dbRow));
    }

    [Test]
    public async Task CleaningEvent_InsertAsync_PrimaryKeyUnique_SuccessfullyInserted()
    {
        var instanceToInsert = new CleaningEvent
        {
            Id = Guid.NewGuid(),
            PollutedLocationId = Guid.NewGuid(),
            Notes = "Harry Potter",
            StartTime = DateTime.UtcNow.ToUniversalTime()
        };

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var eventDbRepository = new CleaningEventDatabaseRepository(context);

        Assert.DoesNotThrowAsync(async () => await eventDbRepository.InsertAsync(instanceToInsert));

        var events = (await eventDbRepository.GetAllAsync()).ToArray();
        Assert.That(events.Length, Is.EqualTo(DbInitializer.FakeCleaningEvents.Length + 1));
        Assert.That(events.Select(x => x.Id).Contains(instanceToInsert.Id), Is.True);
    }

    [Test]
    public async Task CleaningEvent_UpdateAsync_InstanceDoesNotExist_Throws()
    {
        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new CleaningEventDatabaseRepository(context);

        var eventToUpdate = new CleaningEvent
        {
            Id = Guid.NewGuid()
        };

        Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await dbRepository.UpdateAsync(eventToUpdate));
    }

    [Test]
    public async Task CleaningEvent_UpdateAsync_InstanceExists_SuccessfullyUpdated()
    {
        // Get an existing record from the database
        var dbRow = DbInitializer.FakeCleaningEvents.TakeLast(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new CleaningEventDatabaseRepository(context);

        dbRow.Notes = "test notes";
        dbRow.StartTime = new DateTime(2023, 01, 02);

        Assert.DoesNotThrowAsync(async () => await dbRepository.UpdateAsync(dbRow));

        var updatedObject = await dbRepository.GetByPropertyAsync(x => x.Id == dbRow.Id);

        Assert.That(updatedObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(updatedObject.Id, Is.EqualTo(dbRow.Id));
            Assert.That(updatedObject.Notes, Is.EqualTo(dbRow.Notes));
            Assert.That(updatedObject.StartTime, Is.EqualTo(dbRow.StartTime));
            Assert.That(updatedObject.PollutedLocationId, Is.EqualTo(dbRow.PollutedLocationId));
        });
    }

    [Test]
    public async Task CleaningEvent_DeleteAsync_InstanceDoesNotExist_Throws()
    {
        // Try to delete a newly created instance
        var instanceToDelete = new CleaningEvent();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(_options);
        var dbRepository = new CleaningEventDatabaseRepository(context);

        Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await dbRepository.DeleteAsync(instanceToDelete));
    }

    [Test]
    public async Task CleaningEvent_DeleteAsync_InstanceExists_SuccessfullyDeleted_PollutedLocationDoesNotInclude()
    {
        // Try to delete one of the values that was inserted in [SetUp]
        var dbRowId = DbInitializer.FakeCleaningEvents.Skip(3).Take(1).First().Id;

        // Use a clean instance of the context to run the test
        await using var context1 = new PollutedLocationContext(_options);
        await using var context2 = new PollutedLocationContext(_options);
        var locationDbRepository = new PollutedLocationDatabaseRepository(context1); 
        var eventDbRepository = new CleaningEventDatabaseRepository(context2);

        var dbRow = await eventDbRepository.GetByPropertyAsync(x => x.Id == dbRowId);

        Assert.That(dbRow, Is.Not.Null);
        Assert.DoesNotThrowAsync(async () => await eventDbRepository.DeleteAsync(dbRow));

        Assert.That(await eventDbRepository.GetByPropertyAsync(x => x.Id == dbRow.Id), Is.Null);

        var pollutedLocation = await locationDbRepository.GetByPropertyAsync(x => x.Id == dbRow.PollutedLocationId);

        Assert.That(pollutedLocation, Is.Not.Null);
        Assert.That(pollutedLocation.Events.Select(x => x.Id), Does.Not.Contain(dbRow.Id));
    }
    #endregion
}