using Apsitvarkom.Models;

namespace Apsitvarkom.UnitTests.Models;

public class PollutedLocationTests
{
    [Test]
    [TestCase(-1)]
    [TestCase(101)]
    public void Constructor_InvalidProgressPassed_ThrowsArgumentOutOfRangeException(int progress) =>
        Assert.That(Assert.Throws<ArgumentOutOfRangeException>(() => new PollutedLocation { Id = Guid.NewGuid().ToString(), Progress = progress}).ParamName,
            Is.EqualTo("Progress"));

    [Test]
    [TestCase(0)]
    [TestCase(-5)]
    public void Constructor_InvalidRadiusPassed_ThrowsArgumentOutOfRangeException(int radius) =>
        Assert.That(Assert.Throws<ArgumentOutOfRangeException>(() => new PollutedLocation { Id = Guid.NewGuid().ToString(), Radius = radius }).ParamName,
            Is.EqualTo("Radius"));

    [Test]
    public void Constructor_HappyPath_ValidArgumentsPassed_IsSuccess() =>
        Assert.That(
            new PollutedLocation
            {
                Id = Guid.NewGuid().ToString(),
                Location = new Location
                {
                    Latitude = 12.0,
                    Longitude = -140.0
                },
                Radius = 4,
                Severity = Enumerations.LocationSeverityLevel.High,
                Spotted = DateTime.Now,
                Progress = 57,
                Notes = "Lorem ipsum"
            }, Is.Not.Null);
}