using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Moq;
using MockQueryable.Moq;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.UnitTests.DataAccess;

public class PollutedLocationDatabaseRepositoryTests
{
    private Mock<IPollutedLocationContext> m_mockContext = null!;

    [SetUp]
    public void SetUp()
    {
        m_mockContext = new Mock<IPollutedLocationContext>();
    }

    #region Constructor tests
    [Test]
    public void PollutedLocationDatabaseRepositoryConstructor_HappyPath() =>
        Assert.DoesNotThrow(() => new PollutedLocationDatabaseRepository(m_mockContext.Object));
    #endregion

    #region GetAllAsync tests
    [Test]
    public async Task GetAllAsync_DbContextHasNoInstances_EmptyListReturned()
    {
        var dbRows = new List<PollutedLocation>();
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        var dataManager = new PollutedLocationDatabaseRepository(m_mockContext.Object);

        var instances = (await dataManager.GetAllAsync()).ToArray();

        Assert.That(instances, Is.Empty);
    }

    [Test]
    public async Task GetAllAsync_DbContextHasOneInstance_SingleInstanceReturnedWithCorrectProperties()
    {
        var pollutedLocationInstance = new PollutedLocation 
        { 
            Id = Guid.NewGuid(),
            Location = {
                Coordinates = { Latitude = 47.12, Longitude = -41.1251 }
            }, 
            Radius = 35, 
            Severity = PollutedLocation.SeverityLevel.Moderate, 
            Spotted = new DateTime(2022, 11, 12, 19, 23, 30), 
            Notes = "notes", 
            Progress = 67
        };
        var dbRows = new List<PollutedLocation> { pollutedLocationInstance };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        var dataManager = new PollutedLocationDatabaseRepository(m_mockContext.Object);

        var instances = (await dataManager.GetAllAsync()).ToArray();
        var instance = instances.FirstOrDefault();

        Assert.That(instances, Has.Length.EqualTo(1));
        Assert.That(instance, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(instance.Id, Is.EqualTo(pollutedLocationInstance.Id));
            Assert.That(instance.Location.Coordinates.Latitude, Is.EqualTo(pollutedLocationInstance.Location.Coordinates.Latitude));
            Assert.That(instance.Location.Coordinates.Longitude, Is.EqualTo(pollutedLocationInstance.Location.Coordinates.Longitude));
            Assert.That(instance.Radius, Is.EqualTo(pollutedLocationInstance.Radius));
            Assert.That(instance.Severity, Is.EqualTo(pollutedLocationInstance.Severity));
            Assert.That(instance.Spotted, Is.EqualTo(pollutedLocationInstance.Spotted));
            Assert.That(instance.Progress, Is.EqualTo(pollutedLocationInstance.Progress));
            Assert.That(instance.Notes, Is.EqualTo(pollutedLocationInstance.Notes));
        });
    }

    [Test]
    public async Task GetAllAsync_DbContextHasSeveralInstances_InstancesReturnedInCorrectOrder()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var dbRows = DbInitializer.FakePollutedLocations.Value;
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        var dataManager = new PollutedLocationDatabaseRepository(m_mockContext.Object);

        var instances = (await dataManager.GetAllAsync()).ToArray();
        var instanceIds = instances.Select(x => x.Id);

        Assert.That(instances, Has.Length.EqualTo(dbRows.Length));
        Assert.That(instanceIds, Is.EqualTo(dbRows.Select(x => x.Id)));
    }

    [Test]
    public async Task GetAllAsyncOrdered_OrderByDistanceToGivenLocation_InstancesReturnedInCorrectOrder()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();
        var id4 = Guid.NewGuid();
        var dbRows = new List<PollutedLocation>
        {
            new() { Id = id1, Location = { Coordinates = { Latitude = 3, Longitude = 3 } } },
            new() { Id = id2, Location = { Coordinates = { Latitude = 1, Longitude = 1 } } },
            new() { Id = id3, Location = { Coordinates = { Latitude = 4, Longitude = 4 } } },
            new() { Id = id4, Location = { Coordinates = { Latitude = 2, Longitude = 2 } } }
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        var dataManager = new PollutedLocationDatabaseRepository(m_mockContext.Object);

        var orderInRelationToPosition = new Coordinates { Latitude = 0, Longitude = 0 };

        var instances = (await dataManager.GetAllAsync(orderInRelationToPosition)).ToArray();
        var instanceIds = instances.Select(x => x.Id);

        Assert.That(instances, Has.Length.EqualTo(4));
        Assert.That(instanceIds, Is.EqualTo(new[] { id2, id4, id1, id3 }));
    }
    #endregion

    #region GetByPropertyAsync tests
    [Test]
    public async Task GetByPropertyAsync_InstanceWithRequestedPropertyNotFound_NullReturned()
    {
        var dbRows = new List<PollutedLocation>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        var dataManager = new PollutedLocationDatabaseRepository(m_mockContext.Object);

        var instance = await dataManager.GetByPropertyAsync(x => x.Id == Guid.NewGuid());

        Assert.That(instance, Is.Null);
    }

    [Test]
    public async Task GetByPropertyAsync_SingleInstanceWithPropertyFound_InstanceReturned()
    {
        var id = Guid.NewGuid();
        var notes = "123";
        var dbRows = new List<PollutedLocation>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = id, Notes = notes },
            new() { Id = Guid.NewGuid() }
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        var dataManager = new PollutedLocationDatabaseRepository(m_mockContext.Object);

        var instance = await dataManager.GetByPropertyAsync(x => x.Notes == notes);

        Assert.That(instance, Is.Not.Null);
        Assert.That(instance?.Id, Is.EqualTo(id));
    }

    [Test]
    public async Task GetByPropertyAsync_SeveralInstancesWithPropertyFound_FirstMatchingInstanceReturned()
    {
        var id = Guid.NewGuid();
        var dbRows = new List<PollutedLocation>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = id, Severity = PollutedLocation.SeverityLevel.Low },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Severity = PollutedLocation.SeverityLevel.Low}
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        var dataManager = new PollutedLocationDatabaseRepository(m_mockContext.Object);

        var instance = await dataManager.GetByPropertyAsync(x => x.Severity == PollutedLocation.SeverityLevel.Low);

        Assert.That(instance, Is.Not.Null);
        Assert.That(instance?.Id, Is.EqualTo(id));
    }
    #endregion

    #region InsertAsync tests
    [Test]
    public async Task InsertAsync_OneInstanceInserted_InstanceFoundInDbSet()
    {
        var dbRows = new List<PollutedLocation>
        {
            new()
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        m_mockContext.Setup(m => m.Instance).Returns(new Mock<DbContext>().Object);
        var dataManager = new PollutedLocationDatabaseRepository(m_mockContext.Object);

        _ = await dataManager.InsertAsync(dbRows[0]);

        Assert.Multiple(() =>
        {
            Assert.That(m_mockContext.Object.PollutedLocations.Count(), Is.EqualTo(1));
            Assert.That(m_mockContext.Object.PollutedLocations.Contains(dbRows[0]), Is.True);
        });
    }

    [Test]
    public async Task InsertAsync_OneInstanceInserted_InstanceFoundInResult()
    {
        var dbRows = new List<PollutedLocation>
        {
            new()
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        m_mockContext.Setup(m => m.Instance).Returns(new Mock<DbContext>().Object);
        var dataManager = new PollutedLocationDatabaseRepository(m_mockContext.Object);

        var result = await dataManager.InsertAsync(dbRows[0]);

        Assert.That(result, Is.EqualTo(dbRows[0]));
    }
    #endregion
}