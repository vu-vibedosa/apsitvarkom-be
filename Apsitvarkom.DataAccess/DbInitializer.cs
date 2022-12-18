using Apsitvarkom.Models;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Responsible for initializing fake data to databases.
/// </summary>
public static class DbInitializer
{
    public static PollutedLocation[] FakePollutedLocations => InitializeFakePollutedLocationsArray();
    public static CleaningEvent[] FakeCleaningEvents => InitializeFakeCleaningEventsArray();

    public static async Task<int> InitializePollutedLocations(IPollutedLocationContext context)
    {
        await context.PollutedLocations.AddRangeAsync(FakePollutedLocations);
        return await context.Instance.SaveChangesAsync();
    }

    private static PollutedLocation[] InitializeFakePollutedLocationsArray()
    {
        return new PollutedLocation[]
        {
            new()
            {
                Id = Guid.Parse("02f16033-232b-42e4-bfe7-1f9a223a1446"),
                Location =
                {
                    Title = new("Lukiškių skg. 6, 01108 Vilnius, Lithuania", "Lukiškių skg. 6, 01108 Vilnius, Lietuva"),
                    Coordinates =
                    {
                        Latitude = 54.691452,
                        Longitude = 25.266276
                    }
                },
                Radius = 5,
                Severity = PollutedLocation.SeverityLevel.Moderate,
                Spotted = DateTime.UtcNow.AddMinutes(-4).ToUniversalTime(),
                Progress = 0,
                Notes = "Prisoners shattered a window.",
                Events = new List<CleaningEvent>()
            },
            new()
            {
                Id = Guid.Parse("461911ac-ff85-41f8-860a-be0240f0653f"),
                Location =
                {
                    Title = new("VU, matematikos ir informatikos fakultetas, Naugarduko g. 24, 03225 Vilnius, Lithuania", "VU, matematikos ir informatikos fakultetas, Naugarduko g. 24, 03225 Vilnius, Lietuva"),
                    Coordinates =
                    {
                        Latitude = 54.675369,
                        Longitude = 25.273316
                    }
                },
                Radius = 1,
                Severity = PollutedLocation.SeverityLevel.Low,
                Spotted = DateTime.UtcNow.AddMinutes(-3).ToUniversalTime(),
                Progress = 0,
                Notes = "A lot of cigarettes waste on the pavement.",
                Events = new List<CleaningEvent>
                {
                    FakeCleaningEvents[0]
                }
            },
            new()
            {
                Id = Guid.Parse("b2ed322d-331b-4f28-9dbf-4a71dce7504e"),
                Location =
                {
                    Title = new("Didlaukio g. 47, 08303 Vilnius, Lithuania", "Didlaukio g. 47, 08303 Vilnius, Lietuva"),
                    Coordinates =
                    {
                        Latitude = 54.728796,
                        Longitude = 25.264199
                    }
                },
                Radius = 21,
                Severity = PollutedLocation.SeverityLevel.High,
                Spotted = DateTime.UtcNow.AddMinutes(-2).ToUniversalTime(),
                Progress = 0,
                Events = new List<CleaningEvent>()
            },
            new()
            {
                Id = Guid.Parse("bdd6bfe1-85ec-4de5-b0e3-2e5480ef1ee0"),
                Location =
                {
                    Title = new("S. Darius and S. Girėnas Airport, Veiverių g. 132, 46337 Kaunas, Lithuania", "S.Dariaus ir S.Girėno aerodromas, Veiverių g. 132, 46337 Kaunas, Lietuva"),
                    Coordinates =
                    {
                        Latitude = 54.878315,
                        Longitude = 23.883123
                    }
                },
                Radius = 11,
                Severity = PollutedLocation.SeverityLevel.Low,
                Spotted = DateTime.UtcNow.AddMinutes(-1).ToUniversalTime(),
                Progress = 0,
                Notes = "Apsitvarkom to the moooooon",
                Events = new List<CleaningEvent>
                {
                    FakeCleaningEvents[2]
                }
            },
            new()
            {
                Id = Guid.Parse("65f52593-8507-4474-a522-188a2dc53208"),
                Location =
                {
                    Title = new("Karaliaus Mindaugo pr. 50, 44307 Kaunas, Lithuania", "Karaliaus Mindaugo pr. 50, 44307 Kaunas, Lietuva"),
                    Coordinates =
                    {
                        Latitude = 54.891692,
                        Longitude = 23.914362
                    }
                },
                Radius = 150,
                Severity = PollutedLocation.SeverityLevel.Low,
                Spotted = DateTime.UtcNow.ToUniversalTime(),
                Progress = 0,
                Notes = "After the celebration of the latest Euroleague trophy, Zalgiris fans have left the grass trashy.",
                Events = new List<CleaningEvent>
                {
                    FakeCleaningEvents[3]
                }
            },
            new()
            {
                Id = Guid.Parse("d37c6b91-6363-44ce-99a8-2f15287cc5ab"),
                Location =
                {
                    Title = new("Arsenalo g. 5, 01143 Vilnius, Lithuania", "Arsenalo g. 5, 01143 Vilnius, Lietuva"),
                    Coordinates =
                    {
                        Latitude = 54.686762,
                        Longitude = 25.291317
                    }
                },
                Radius = 10,
                Severity = PollutedLocation.SeverityLevel.High,
                Spotted = DateTime.UtcNow.ToUniversalTime(),
                Progress = 0,
                Notes = "The Vilnius Castle has slipped off the mountain.",
                Events = new List<CleaningEvent>
                {
                    FakeCleaningEvents[5]
                }
            },
            new()
            {
                Id = Guid.Parse("151757e9-fce3-4bb3-93db-08b93d71245e"),
                Location =
                {
                    Title = new("Pajuostės pl. 27b, 36102 Panevėžys, Lithuania", "Pajuostės pl. 27b, 36102 Panevėžys, Lietuva"),
                    Coordinates =
                    {
                        Latitude = 55.730551,
                        Longitude = 24.394250
                    }
                },
                Radius = 50,
                Severity = PollutedLocation.SeverityLevel.Low,
                Spotted = DateTime.UtcNow.ToUniversalTime(),
                Progress = 0,
                Notes = "Couldn't manage to grab all the beer cans I found on the beach.",
                Events = new List<CleaningEvent>()
            },
            new()
            {
                Id = Guid.Parse("9de943d3-3ac6-4c55-adcf-fc6aa79b0597"),
                Location =
                {
                    Title = new("Senoji perkėla (Šiaurinis ragas), 93100 Klaipėda, Lithuania", "Senoji perkėla (Šiaurinis ragas), 93100 Klaipėda, Lietuva"),
                    Coordinates =
                    {
                        Latitude = 55.705656,
                        Longitude = 21.122825
                    }
                },
                Radius = 200,
                Severity = PollutedLocation.SeverityLevel.Low,
                Spotted = DateTime.UtcNow.ToUniversalTime(),
                Progress = 0,
                Notes = "Maybe we should tidy this up as the tourists see this place first of our whole city.",
                Events = new List<CleaningEvent>
                {
                    FakeCleaningEvents[4]
                }
            },
            new()
            {
                Id = Guid.Parse("dc1513da-a60b-49e5-adba-d0aeed77f125"),
                Location =
                {
                    Title = new("Sodininkų g. 20, 89327 Mažeikiai, Lithuania", "Sodininkų g. 5, 89327 Mažeikiai, Lietuva"),
                    Coordinates =
                    {
                        Latitude = 56.293939,
                        Longitude = 22.340248
                    }
                },
                Radius = 50,
                Severity = PollutedLocation.SeverityLevel.Moderate,
                Spotted = DateTime.UtcNow.ToUniversalTime(),
                Progress = 0,
                Notes = "The fans made a big mess after the game. Apsitvarkom?",
                Events = new List<CleaningEvent>
                {
                    FakeCleaningEvents[1]
                }
            },
            new()
            {
                Id = Guid.Parse("872c1fd2-c640-4bb3-bab4-74cdfb2a53cd"),
                Location =
                {
                    Title = new("Vydūno g. 19, 74122 Jurbarkas, Lithuania", "Vydūno g. 21, 74122 Jurbarkas, Lietuva"),
                    Coordinates =
                    {
                        Latitude = 55.0793004,
                        Longitude = 22.7563897
                    }
                },
                Radius = 50,
                Severity = PollutedLocation.SeverityLevel.Low,
                Spotted = DateTime.UtcNow.ToUniversalTime(),
                Progress = 0,
                Notes = "So much confetti laying around after the town event.",
                Events = new List<CleaningEvent>()
            }
        };
    }

    private static CleaningEvent[] InitializeFakeCleaningEventsArray()
    {
        return new CleaningEvent[]
        {
            new()
            {
                Id = Guid.Parse("3621f801-00c6-48a6-9dd4-15a0f2fdb7bb"),
                StartTime = DateTime.UtcNow.AddMinutes(3).ToUniversalTime(),
                Notes = "Let's patch this place up.",
                IsFinalized = false
            },
            new()
            {
                Id = Guid.Parse("073d1855-1dba-4ce6-857b-3cfa9f36a1ba"),
                StartTime = DateTime.UtcNow.AddHours(15).ToUniversalTime(),
                Notes = "Bring your own trash-bags.",
                IsFinalized = false
            },
            new()
            {
                Id = Guid.Parse("8e8bf1df-e732-409e-976a-d61806ee7c19"),
                StartTime = DateTime.UtcNow.AddDays(3).ToUniversalTime(),
                IsFinalized = false
            },
            new()
            {
                Id = Guid.Parse("0d9374dc-0d28-4b7c-86bf-4cc36e848604"),
                StartTime = DateTime.UtcNow.AddDays(31).ToUniversalTime(),
                Notes = "Apsitvarkom!:)",
                IsFinalized = false
            },
            new()
            {
                Id = Guid.Parse("5638be6e-773c-405d-a7ef-1f76115ae8c5"),
                StartTime = DateTime.UtcNow.AddDays(12).ToUniversalTime(),
                Notes = "Let's finish it once and for all.",
                IsFinalized = false
            },
            new()
            {
                Id = Guid.Parse("2931a606-c344-4ffd-8774-0cc07d859902"),
                StartTime = DateTime.UtcNow.AddDays(1.5).ToUniversalTime(),
                IsFinalized = false
            }
        };
    }
}