using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.DTO;
using Apsitvarkom.Models.Mapping;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.IntegrationTests.DataAccess;

public class DatabaseTests
{
    private IMapper m_mapper = null!;
    private DbContextOptions<PollutedLocationContext> m_options = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<PollutedLocationProfile>(); });
        config.AssertConfigurationIsValid();
        m_mapper = config.CreateMapper();
    }

    [SetUp]
    public async Task SetUp()
    {
        m_options = new DbContextOptionsBuilder<PollutedLocationContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        // Insert seed data into the database using one instance of the context
        await using var context = new PollutedLocationContext(m_options);
        DbInitializer.InitializePollutedLocations(context);
    }

    [Test]
    public async Task GetAllTest()
    {
        var instanceGuids = DbInitializer.FakePollutedLocations.Value.Select(location => location.Id.ToString()).ToArray();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(m_options);
        var dbRepository = new PollutedLocationDTODatabaseRepository(context, m_mapper);
        var response = (await dbRepository.GetAllAsync()).ToArray();

        Assert.That(response.Length, Is.EqualTo(instanceGuids.Length));
        Assert.That(response.Select(x => x.Id), Is.EqualTo(instanceGuids));
    }

    [Test]
    public async Task GetAllSortedTest()
    {
        var instanceGuids = DbInitializer.FakePollutedLocations.Value.Select(location => location.Id.ToString()).ToArray();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(m_options);
        var dbRepository = new PollutedLocationDTODatabaseRepository(context, m_mapper);
        var location = new Location { Coordinates = new Coordinates { Latitude = 12.123, Longitude = -12.123 } };
        var response = (await dbRepository.GetAllAsync(location)).ToArray();

        Assert.That(response.Length, Is.EqualTo(instanceGuids.Length));
        Assert.That(response.Select(x => x.Id), Is.Not.EqualTo(instanceGuids));
        Assert.That(response.Select(x => x.Id), Is.EquivalentTo(instanceGuids));
    }

    [Test]
    public async Task GetByPropertyTest()
    {
        var dbRow = DbInitializer.FakePollutedLocations.Value.Skip(3).Take(1).Single();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(m_options);
        var dbRepository = new PollutedLocationDTODatabaseRepository(context, m_mapper);
        
        var response = (await dbRepository.GetByPropertyAsync(x => x.Id == dbRow.Id.ToString()));
        
        Assert.That(response, Is.Not.Null);
        var instance = m_mapper.Map<PollutedLocationDTO, PollutedLocation>(response!);
        Assert.Multiple(() =>
        {
            Assert.That(instance.Id, Is.EqualTo(dbRow.Id));
            Assert.That(instance.Coordinates.Latitude, Is.EqualTo(dbRow.Coordinates.Latitude));
            Assert.That(instance.Coordinates.Longitude, Is.EqualTo(dbRow.Coordinates.Longitude));
            Assert.That(instance.Radius, Is.EqualTo(dbRow.Radius));
            Assert.That(instance.Severity, Is.EqualTo(dbRow.Severity));
            Assert.That(instance.Spotted, Is.EqualTo(dbRow.Spotted));
            Assert.That(instance.Progress, Is.EqualTo(dbRow.Progress));
            Assert.That(instance.Notes, Is.EqualTo(dbRow.Notes));
        });
    }
}