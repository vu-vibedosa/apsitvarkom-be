using Apsitvarkom.Models;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Responsible for initializing fake data to databases
/// </summary>
public static class DbInitializer
{
    public static void InitializePollutedLocations(PollutedLocationContext context)
    {
        if (context.PollutedLocations.Any())
        {
            return;
        }

        var fakePollutedLocations = new PollutedLocation[]
        {
            new()
            {
                Id = new Guid("02f16033-232b-42e4-bfe7-1f9a223a1446"),
                Coordinates = new()
                {
                    Latitude = 54.691452,
                    Longitude = 25.266276
                },
                Radius = 5,
                Severity = Enumerations.LocationSeverityLevel.Moderate,
                Spotted = DateTime.Parse("2019-08-23T14:05:43Z").ToUniversalTime(),
                Progress = 41,
                Notes = "Prisoners broke a window."
            },
            new()
            {
                Id = new Guid("461911ac-ff85-41f8-860a-be0240f0653f"),
                Coordinates = new()
                {
                    Latitude = 54.675369,
                    Longitude = 25.273316
                },
                Radius = 1,
                Severity = Enumerations.LocationSeverityLevel.Low,
                Spotted = DateTime.Parse("2023-04-13T07:16:55Z").ToUniversalTime(),
                Progress = 13,
                Notes = "A lot of cigarettes waste on the pavement."
            },
            new()
            {
                Id = new Guid("b2ed322d-331b-4f28-9dbf-4a71dce7504e"),
                Coordinates = new()
                {
                    Latitude = 54.728796,
                    Longitude = 25.264199
                },
                Radius = 21,
                Severity = Enumerations.LocationSeverityLevel.High,
                Spotted = DateTime.Parse("2023-03-12T23:41:21Z").ToUniversalTime(),
                Progress = 80,
            },
            new()
            {
                Id = new Guid("bdd6bfe1-85ec-4de5-b0e3-2e5480ef1ee0"),
                Coordinates = new()
                {
                    Latitude = 54.878315,
                    Longitude = 23.883123
                },
                Radius = 11,
                Severity = Enumerations.LocationSeverityLevel.Low,
                Spotted = DateTime.Parse("2023-11-11T11:11:11Z").ToUniversalTime(),
                Progress = 11,
                Notes = "Apsitvarkom to the moooooon"
            },
            new()
            {
                Id = new Guid("65f52593-8507-4474-a522-188a2dc53208"),
                Coordinates = new()
                {
                    Latitude = 54.891692,
                    Longitude = 23.914362
                },
                Radius = 150,
                Severity = Enumerations.LocationSeverityLevel.Low,
                Spotted = DateTime.Parse("2023-07-11T02:13:14Z").ToUniversalTime(),
                Progress = 0,
                Notes = "After the celebration of the latest Euroleague trophy, Zalgiris fans have left the grass trashy."
            },
            new()
            {
                Id = new Guid("d37c6b91-6363-44ce-99a8-2f15287cc5ab"),
                Coordinates = new()
                {
                    Latitude = 54.686762,
                    Longitude = 25.291317
                },
                Radius = 10,
                Severity = Enumerations.LocationSeverityLevel.High,
                Spotted = DateTime.Parse("2023-01-01T04:00:01Z").ToUniversalTime(),
                Progress = 0,
                Notes = "The Vilnius Castle has slipped off the mountain."
            },
            new()
            {
                Id = new Guid("151757e9-fce3-4bb3-93db-08b93d71245e"),
                Coordinates = new()
                {
                    Latitude = 55.730551,
                    Longitude = 24.394250
                },
                Radius = 50,
                Severity = Enumerations.LocationSeverityLevel.Low,
                Spotted = DateTime.Parse("2022-06-08T01:12:23Z").ToUniversalTime(),
                Progress = 80,
                Notes = "Couldn't manage to grab all the beer cans I found on the beach."
            },
            new()
            {
                Id = new Guid("9de943d3-3ac6-4c55-adcf-fc6aa79b0597"),
                Coordinates = new()
                {
                    Latitude = 55.705656,
                    Longitude = 21.122825
                },
                Radius = 200,
                Severity = Enumerations.LocationSeverityLevel.Low,
                Spotted = DateTime.Parse("2023-04-01T13:14:15Z").ToUniversalTime(),
                Progress = 0,
                Notes = "Maybe we should tidy this up as the tourists see this place first of our whole city."
            },
            new()
            {
                Id = new Guid("dc1513da-a60b-49e5-adba-d0aeed77f125"),
                Coordinates = new()
                {
                    Latitude = 56.293939,
                    Longitude = 22.340248
                },
                Radius = 50,
                Severity = Enumerations.LocationSeverityLevel.Moderate,
                Spotted = DateTime.Parse("2022-06-20T11:22:33Z").ToUniversalTime(),
                Progress = 20,
                Notes = "The fans made a big mess after the game. Apsitvarkom?"
            },
        };

        context.PollutedLocations.AddRange(fakePollutedLocations);
        context.SaveChanges();
    }
}
