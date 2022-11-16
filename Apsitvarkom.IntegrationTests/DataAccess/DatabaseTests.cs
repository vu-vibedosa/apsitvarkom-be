﻿using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.IntegrationTests.DataAccess;

public class DatabaseTests
{
    private DbContextOptions<PollutedLocationContext> m_options = null!;

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
        var instanceGuids = DbInitializer.FakePollutedLocations.Value.Select(location => location.Id).ToArray();

        // Use a clean instance of the context to run the test
        await using var context = new PollutedLocationContext(m_options);
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
        await using var context = new PollutedLocationContext(m_options);
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
        await using var context = new PollutedLocationContext(m_options);
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
}