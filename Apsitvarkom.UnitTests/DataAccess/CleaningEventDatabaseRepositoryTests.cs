using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace Apsitvarkom.UnitTests.DataAccess;

public class CleaningEventDatabaseRepositoryTests
{
    private Mock<IPollutedLocationContext> _mockContext = null!;

    [SetUp]
    public void SetUp()
    {
        _mockContext = new Mock<IPollutedLocationContext>();
    }

    #region Constructor tests
    [Test]
    public void CleaningEventDatabaseRepositoryConstructor_HappyPath() =>
        Assert.DoesNotThrow(() => new CleaningEventDatabaseRepository(_mockContext.Object));
    #endregion

    #region GetAllAsync tests
    [Test]
    public async Task GetAllAsync_DbContextHasNoInstances_EmptyListReturned()
    {
        var dbRows = new List<CleaningEvent>();
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(m => m.CleaningEvents).Returns(mock.Object);
        var dataManager = new CleaningEventDatabaseRepository(_mockContext.Object);

        var instances = (await dataManager.GetAllAsync()).ToArray();

        Assert.That(instances, Is.Empty);
    }

    [Test]
    public async Task GetAllAsync_DbContextHasOneInstance_SingleInstanceReturnedWithCorrectProperties()
    {
        var cleaningEventInstance = new CleaningEvent
        {
            Id = Guid.NewGuid(),
            PollutedLocationId = Guid.NewGuid(),
            StartTime = new DateTime(2022, 11, 12, 19, 23, 30),
            Notes = "notes"
        };
        var dbRows = new List<CleaningEvent> { cleaningEventInstance };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(m => m.CleaningEvents).Returns(mock.Object);
        var dataManager = new CleaningEventDatabaseRepository(_mockContext.Object);

        var instances = (await dataManager.GetAllAsync()).ToArray();
        var instance = instances.FirstOrDefault();

        Assert.That(instances, Has.Length.EqualTo(1));
        Assert.That(instance, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(instance.Id, Is.EqualTo(cleaningEventInstance.Id));
            Assert.That(instance.PollutedLocationId, Is.EqualTo(cleaningEventInstance.PollutedLocationId));
            Assert.That(instance.StartTime, Is.EqualTo(cleaningEventInstance.StartTime));
            Assert.That(instance.Notes, Is.EqualTo(cleaningEventInstance.Notes));
        });
    }

    [Test]
    public async Task GetAllAsync_DbContextHasSeveralInstances_InstancesReturnedInCorrectOrder()
    {
        var dbRows = DbInitializer.FakeCleaningEvents;
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(m => m.CleaningEvents).Returns(mock.Object);
        var dataManager = new CleaningEventDatabaseRepository(_mockContext.Object);

        var instances = (await dataManager.GetAllAsync()).ToArray();
        var instanceIds = instances.Select(x => x.Id);

        Assert.That(instances, Has.Length.EqualTo(dbRows.Length));
        Assert.That(instanceIds, Is.EqualTo(dbRows.Select(x => x.Id)));
    }
    #endregion

    #region GetByPropertyAsync tests
    [Test]
    public async Task GetByPropertyAsync_InstanceWithRequestedPropertyNotFound_NullReturned()
    {
        var dbRows = new List<CleaningEvent>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(m => m.CleaningEvents).Returns(mock.Object);
        var dataManager = new CleaningEventDatabaseRepository(_mockContext.Object);

        var instance = await dataManager.GetByPropertyAsync(x => x.Id == Guid.NewGuid());

        Assert.That(instance, Is.Null);
    }

    [Test]
    public async Task GetByPropertyAsync_SingleInstanceWithPropertyFound_InstanceReturned()
    {
        var id = Guid.NewGuid();
        var notes = "123";
        var dbRows = new List<CleaningEvent>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = id, Notes = notes },
            new() { Id = Guid.NewGuid() }
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(m => m.CleaningEvents).Returns(mock.Object);
        var dataManager = new CleaningEventDatabaseRepository(_mockContext.Object);

        var instance = await dataManager.GetByPropertyAsync(x => x.Notes == notes);

        Assert.That(instance, Is.Not.Null);
        Assert.That(instance?.Id, Is.EqualTo(id));
    }

    [Test]
    public async Task GetByPropertyAsync_SeveralInstancesWithPropertyFound_FirstMatchingInstanceReturned()
    {
        var id = Guid.NewGuid();
        var matchingPollutedLocationId = Guid.NewGuid();
        var dbRows = new List<CleaningEvent>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = id, PollutedLocationId = matchingPollutedLocationId },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), PollutedLocationId = matchingPollutedLocationId }
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(m => m.CleaningEvents).Returns(mock.Object);
        var dataManager = new CleaningEventDatabaseRepository(_mockContext.Object);

        var instance = await dataManager.GetByPropertyAsync(x => x.PollutedLocationId == matchingPollutedLocationId);

        Assert.That(instance, Is.Not.Null);
        Assert.That(instance?.Id, Is.EqualTo(id));
    }
    #endregion

    #region GetByPropertyAsync tests
    [Test]
    public async Task ExistsByPropertyAsync_InstanceWithRequestedPropertyNotFound_FalseReturned()
    {
        var dbRows = new List<CleaningEvent>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(m => m.CleaningEvents).Returns(mock.Object);
        var dataManager = new CleaningEventDatabaseRepository(_mockContext.Object);

        var instance = await dataManager.ExistsByPropertyAsync(x => x.Id == Guid.NewGuid());

        Assert.That(instance, Is.False);
    }

    [Test]
    public async Task ExistsByPropertyAsync_AtLeastOneInstanceWithPropertyFound_TrueReturned()
    {
        var notes = "123";
        var dbRows = new List<CleaningEvent>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Notes = notes },
            new() { Id = Guid.NewGuid() }
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(m => m.CleaningEvents).Returns(mock.Object);
        var dataManager = new CleaningEventDatabaseRepository(_mockContext.Object);

        var instance = await dataManager.ExistsByPropertyAsync(x => x.Notes == notes);

        Assert.That(instance, Is.True);
    }
    #endregion

    #region InsertAsync tests
    [Test]
    public async Task InsertAsync_OneInstanceInserted_InstanceFoundInDbSet()
    {
        var dbRows = new List<CleaningEvent>
        {
            new()
        };
        var mock = dbRows.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(m => m.CleaningEvents).Returns(mock.Object);
        _mockContext.Setup(m => m.Instance).Returns(new Mock<DbContext>().Object);
        var dataManager = new CleaningEventDatabaseRepository(_mockContext.Object);

        await dataManager.InsertAsync(dbRows[0]);

        Assert.Multiple(() =>
        {
            Assert.That(_mockContext.Object.CleaningEvents.Count(), Is.EqualTo(1));
            Assert.That(_mockContext.Object.CleaningEvents.Contains(dbRows[0]), Is.True);
        });
    }
    #endregion

    #region UpdateAsync
    [Test]
    public async Task UpdateAsync_InstanceExists_SuccessfullyUpdated()
    {
        var dbRows = new List<CleaningEvent>
        {
            new()
        };

        var mock = dbRows.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(m => m.CleaningEvents).Returns(mock.Object);
        _mockContext.Setup(m => m.Instance).Returns(new Mock<DbContext>().Object);
        var dataManager = new CleaningEventDatabaseRepository(_mockContext.Object);

        await dataManager.UpdateAsync(dbRows[0]);

        Assert.Multiple(() =>
        {
            Assert.That(_mockContext.Object.CleaningEvents.Count(), Is.EqualTo(1));
            Assert.That(_mockContext.Object.CleaningEvents.Contains(dbRows[0]), Is.True);
        });
    }
    #endregion

    #region DeleteAsync
    [Test]
    public async Task DeleteAsync_InstanceExists_SuccessfullyDeleted()
    {
        var dbRows = new List<CleaningEvent>
        {
            new() { Id = new Guid() },
            new() { Id = new Guid() },
            new() { Id = new Guid() }
        };
        var locationToDelete = dbRows.TakeLast(1).Single();
        var mock = dbRows.SkipLast(1).AsQueryable().BuildMockDbSet();
        _mockContext.Setup(m => m.CleaningEvents).Returns(mock.Object);
        _mockContext.Setup(m => m.Instance).Returns(new Mock<DbContext>().Object);
        var dataManager = new CleaningEventDatabaseRepository(_mockContext.Object);

        await dataManager.DeleteAsync(locationToDelete);

        Assert.Multiple(() =>
        {
            Assert.That(_mockContext.Object.CleaningEvents.Count(), Is.EqualTo(2));
            Assert.That(_mockContext.Object.CleaningEvents.Select(x => x.Id), Is.EqualTo(dbRows.SkipLast(1).Select(x => x.Id)));
            Assert.That(_mockContext.Object.CleaningEvents.Contains(locationToDelete), Is.False);
        });
    }
    #endregion
}