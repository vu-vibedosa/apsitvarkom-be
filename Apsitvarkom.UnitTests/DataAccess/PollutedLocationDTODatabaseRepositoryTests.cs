using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using AutoMapper;
using Apsitvarkom.Models.Mapping;
using Moq;
using MockQueryable.Moq;
using System.Globalization;
using static Apsitvarkom.Models.Enumerations;

namespace Apsitvarkom.UnitTests.DataAccess;

public class PollutedLocationDTODatabaseRepositoryTests
{
    private IMapper m_mapper = null!;
    private Mock<IPollutedLocationContext> m_mockContext = null!;


    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PollutedLocationProfile>();
        });
        config.AssertConfigurationIsValid();
        m_mapper = config.CreateMapper();
    }

    [SetUp]
    public void SetUp()
    {
        m_mockContext = new Mock<IPollutedLocationContext>();
    }

    #region Constructor tests
    [Test]
    public void PollutedLocationDTODatabaseRepositoryConstructor_HappyPath() =>
        Assert.DoesNotThrow(() => new PollutedLocationDTODatabaseRepository(m_mockContext.Object, m_mapper));
    #endregion

    #region GetAllAsync tests
    [Test]
    public async Task GetAllAsync_DbContextHasNoInstances_EmptyListReturned()
    {
        var dbRows = new List<PollutedLocation>();
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        var dataManager = new PollutedLocationDTODatabaseRepository(m_mockContext.Object, m_mapper);

        var instances = (await dataManager.GetAllAsync()).ToArray();

        Assert.That(instances, Is.Empty);
    }

    [Test]
    public async Task GetAllAsync_DbContextHasOneInstance_SingleInstanceReturnedWithCorrectProperties()
    {
        var pollutedLocationInstance = new PollutedLocation 
        { 
            Id = Guid.NewGuid(),
            Coordinates = new Coordinates { Latitude = 47.12, Longitude = -41.1251 }, 
            Radius = 35, 
            Severity = LocationSeverityLevel.Moderate, 
            Spotted = new DateTime(2022, 11, 12, 19, 23, 30), 
            Notes = "notes", 
            Progress = 67
        };
        var dbRows = new List<PollutedLocation> { pollutedLocationInstance };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        var dataManager = new PollutedLocationDTODatabaseRepository(m_mockContext.Object, m_mapper);

        var instances = (await dataManager.GetAllAsync()).ToArray();
        var instance = instances.FirstOrDefault();

        Assert.That(instances, Has.Length.EqualTo(1));
        Assert.That(instance, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(instance!.Id, Is.EqualTo(pollutedLocationInstance.Id.ToString()));
            Assert.That(instance.Coordinates?.Latitude, Is.EqualTo(pollutedLocationInstance.Coordinates.Latitude));
            Assert.That(instance.Coordinates?.Longitude, Is.EqualTo(pollutedLocationInstance.Coordinates.Longitude));
            Assert.That(instance.Radius, Is.EqualTo(pollutedLocationInstance.Radius));
            Assert.That(instance.Severity, Is.EqualTo(pollutedLocationInstance.Severity.ToString()));
            Assert.That(instance.Spotted, Is.EqualTo(pollutedLocationInstance.Spotted.ToString(CultureInfo.InvariantCulture)));
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
        var dataManager = new PollutedLocationDTODatabaseRepository(m_mockContext.Object, m_mapper);

        var instances = (await dataManager.GetAllAsync()).ToArray();
        var instanceIds = instances.Select(x => x.Id);

        Assert.That(instances, Has.Length.EqualTo(dbRows.Length));
        Assert.That(instanceIds, Is.EqualTo(dbRows.Select(x => x.Id.ToString())));
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
            new() { Id = id1, Coordinates = new Coordinates {Latitude = 3, Longitude = 3} },
            new() { Id = id2, Coordinates = new Coordinates {Latitude = 1, Longitude = 1} },
            new() { Id = id3, Coordinates = new Coordinates {Latitude = 4, Longitude = 4} },
            new() { Id = id4, Coordinates = new Coordinates {Latitude = 2, Longitude = 2} }
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        var dataManager = new PollutedLocationDTODatabaseRepository(m_mockContext.Object, m_mapper);

        var orderInRelationToPosition = new Location { Coordinates = new Coordinates { Latitude = 0, Longitude = 0 } };

        var instances = (await dataManager.GetAllAsync(orderInRelationToPosition)).ToArray();
        var instanceIds = instances.Select(x => x.Id);

        Assert.That(instances, Has.Length.EqualTo(4));
        Assert.That(instanceIds, Is.EqualTo(new[] {id2.ToString(), id4.ToString(), id1.ToString(), id3.ToString()}));
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
        var dataManager = new PollutedLocationDTODatabaseRepository(m_mockContext.Object, m_mapper);

        var instance = await dataManager.GetByPropertyAsync(x => x.Id == Guid.NewGuid().ToString());

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
        var dataManager = new PollutedLocationDTODatabaseRepository(m_mockContext.Object, m_mapper);

        var instance = await dataManager.GetByPropertyAsync(x => x.Notes == notes);

        Assert.That(instance, Is.Not.Null);
        Assert.That(instance?.Id, Is.EqualTo(id.ToString()));
    }

    [Test]
    public async Task GetByPropertyAsync_SeveralInstancesWithPropertyFound_FirstMatchingInstanceReturned()
    {
        var id = Guid.NewGuid();
        var dbRows = new List<PollutedLocation>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = id, Severity = LocationSeverityLevel.Low },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Severity = LocationSeverityLevel.Low}
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        m_mockContext.Setup(m => m.PollutedLocations).Returns(mock.Object);
        var dataManager = new PollutedLocationDTODatabaseRepository(m_mockContext.Object, m_mapper);

        var instance = await dataManager.GetByPropertyAsync(x => x.Severity == LocationSeverityLevel.Low.ToString());

        Assert.That(instance, Is.Not.Null);
        Assert.That(instance?.Id, Is.EqualTo(id.ToString()));
    }
    #endregion
}