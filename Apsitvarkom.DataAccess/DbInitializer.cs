using Apsitvarkom.Models;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Responsible for initializing fake data to databases
/// </summary>
public static class DbInitializer
{
    public static readonly Lazy<PollutedLocation[]> FakePollutedLocations = new(InitializeFakePollutedLocationsArray);
    public static readonly Lazy<TidyingEvent[]> FakeTidyingEvents = new(InitializeFakeTidyingEventsArray);

    public static Task InitializePollutedLocations(IPollutedLocationContext context)
    {
        context.PollutedLocations.AddRange(FakePollutedLocations.Value);
        context.TidyingEvents.AddRange(FakeTidyingEvents.Value);
        return context.Instance.SaveChangesAsync();
    }

    private static PollutedLocation[] InitializeFakePollutedLocationsArray()
    {
        return new PollutedLocation[]
        {
            new()
            {
                Id = new Guid("02f16033-232b-42e4-bfe7-1f9a223a1446"),
                Location =
                {
                    Title = "Lukiškių kalėjimas",
                    Coordinates =
                    {
                        Latitude = 54.691452,
                        Longitude = 25.266276
                    }
                },
                Radius = 5,
                Severity = PollutedLocation.SeverityLevel.Moderate,
                Spotted = DateTime.Parse("2019-08-23T14:05:43Z").ToUniversalTime(),
                Progress = 41,
                Notes = "Prisoners broke a window."
            },
            new()
            {
                Id = new Guid("461911ac-ff85-41f8-860a-be0240f0653f"),
                Location =
                {
                    Title = "VU, matematikos ir informatikos fakultetas",
                    Coordinates =
                    {
                        Latitude = 54.675369,
                        Longitude = 25.273316
                    }
                },
                Radius = 1,
                Severity = PollutedLocation.SeverityLevel.Low,
                Spotted = DateTime.Parse("2023-04-13T07:16:55Z").ToUniversalTime(),
                Progress = 13,
                Notes = "A lot of cigarettes waste on the pavement."
            },
            new()
            {
                Id = new Guid("b2ed322d-331b-4f28-9dbf-4a71dce7504e"),
                Location =
                {
                    Title = "Didlaukio g. 47",
                    Coordinates =
                    {
                        Latitude = 54.728796,
                        Longitude = 25.264199
                    },
                },
                Radius = 21,
                Severity = PollutedLocation.SeverityLevel.High,
                Spotted = DateTime.Parse("2023-03-12T23:41:21Z").ToUniversalTime(),
                Progress = 80,
            },
            new()
            {
                Id = new Guid("bdd6bfe1-85ec-4de5-b0e3-2e5480ef1ee0"),
                Location =
                {
                    Title = "",
                    Coordinates =
                    {
                        Latitude = 54.878315,
                        Longitude = 23.883123
                    },
                },
                Radius = 11,
                Severity = PollutedLocation.SeverityLevel.Low,
                Spotted = DateTime.Parse("2023-11-11T11:11:11Z").ToUniversalTime(),
                Progress = 11,
                Notes = "Apsitvarkom to the moooooon"
            },
            new()
            {
                Id = new Guid("65f52593-8507-4474-a522-188a2dc53208"),
                Location =
                {
                    Title = "Žalgirio arena",
                    Coordinates =
                    {
                        Latitude = 54.891692,
                        Longitude = 23.914362
                    },
                },
                Radius = 150,
                Severity = PollutedLocation.SeverityLevel.Low,
                Spotted = DateTime.Parse("2023-07-11T02:13:14Z").ToUniversalTime(),
                Progress = 0,
                Notes = "After the celebration of the latest Euroleague trophy, Zalgiris fans have left the grass trashy."
            },
            new()
            {
                Id = new Guid("d37c6b91-6363-44ce-99a8-2f15287cc5ab"),
                Location =
                {
                    Title = "Gedimino pilis",
                    Coordinates =
                    {
                        Latitude = 54.686762,
                        Longitude = 25.291317
                    },
                },
                Radius = 10,
                Severity = PollutedLocation.SeverityLevel.High,
                Spotted = DateTime.Parse("2023-01-01T04:00:01Z").ToUniversalTime(),
                Progress = 0,
                Notes = "The Vilnius Castle has slipped off the mountain."
            },
            new()
            {
                Id = new Guid("151757e9-fce3-4bb3-93db-08b93d71245e"),
                Location =
                {
                    Title = "Panevėžio paplūdimys",
                    Coordinates =
                    {
                        Latitude = 55.730551,
                        Longitude = 24.394250
                    },
                },
                Radius = 50,
                Severity = PollutedLocation.SeverityLevel.Low,
                Spotted = DateTime.Parse("2022-06-08T01:12:23Z").ToUniversalTime(),
                Progress = 80,
                Notes = "Couldn't manage to grab all the beer cans I found on the beach."
            },
            new()
            {
                Id = new Guid("9de943d3-3ac6-4c55-adcf-fc6aa79b0597"),
                Location =
                {
                    Title = "Klaipėdos kruizinių laivų terminalas",
                    Coordinates =
                    {
                        Latitude = 55.705656,
                        Longitude = 21.122825
                    },
                },
                Radius = 200,
                Severity = PollutedLocation.SeverityLevel.Low,
                Spotted = DateTime.Parse("2023-04-01T13:14:15Z").ToUniversalTime(),
                Progress = 0,
                Notes = "Maybe we should tidy this up as the tourists see this place first of our whole city."
            },
            new()
            {
                Id = new Guid("dc1513da-a60b-49e5-adba-d0aeed77f125"),
                Location =
                {
                    Title = "Mažeikių miesto centrinis stadionas",
                    Coordinates =
                    {
                        Latitude = 56.293939,
                        Longitude = 22.340248
                    },
                },
                Radius = 50,
                Severity = PollutedLocation.SeverityLevel.Moderate,
                Spotted = DateTime.Parse("2022-06-20T11:22:33Z").ToUniversalTime(),
                Progress = 20,
                Notes = "The fans made a big mess after the game. Apsitvarkom?"
            },
        };
    }

    private static TidyingEvent[] InitializeFakeTidyingEventsArray()
    {
        return new TidyingEvent[]
        {
            new()
            {
                Id = new Guid("3621f801-00c6-48a6-9dd4-15a0f2fdb7bb"),
                PollutedLocationId = FakePollutedLocations.Value[4].Id,
                StartTime = new DateTime(2022, 12, 23, 23, 00, 00).ToUniversalTime(),
                Notes = "Let's patch this place up."
            },
            new()
            {
                Id = new Guid("073d1855-1dba-4ce6-857b-3cfa9f36a1ba"),
                PollutedLocationId = FakePollutedLocations.Value[4].Id,
                StartTime = new DateTime(2023, 1, 14, 14, 00, 00).ToUniversalTime(),
                Notes = "Bring your own trash-bags."
            },
            new()
            {
                Id = new Guid("8e8bf1df-e732-409e-976a-d61806ee7c19"),
                PollutedLocationId = FakePollutedLocations.Value[0].Id,
                StartTime = new DateTime(2023, 1, 20, 15, 30, 00).ToUniversalTime(),
            },
            new()
            {
                Id = new Guid("0d9374dc-0d28-4b7c-86bf-4cc36e848604"),
                PollutedLocationId = FakePollutedLocations.Value[2].Id,
                StartTime = new DateTime(2022, 12, 20, 16, 00, 00).ToUniversalTime(),
                Notes = "Apsitvarkom!:)"
            },
            new()
            {
                Id = new Guid("5638be6e-773c-405d-a7ef-1f76115ae8c5"),
                PollutedLocationId = FakePollutedLocations.Value[7].Id,
                StartTime = new DateTime(2022, 12, 12, 12, 12, 12).ToUniversalTime(),
                Notes = "Let's finish it for once and for all."
            },
        };
    }
}