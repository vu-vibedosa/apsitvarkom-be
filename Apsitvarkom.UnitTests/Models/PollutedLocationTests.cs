using Apsitvarkom.Models;
using static Apsitvarkom.Models.Enumerations;

namespace Apsitvarkom.UnitTests.Models;

public class PollutedLocationTests
{
    [Test]
    public void Constructor_HappyPath_ValidArgumentsPassed_IsSuccess() =>
        Assert.That(
            new PollutedLocation
            {
                Id = Guid.NewGuid(),
                Location = new Location
                {
                    Latitude = 12.0,
                    Longitude = -140.0
                },
                Radius = 4,
                Severity = LocationSeverityLevel.High,
                Spotted = DateTime.Now,
                Progress = 57,
                Notes = "Lorem ipsum"
            }, Is.Not.Null);
}