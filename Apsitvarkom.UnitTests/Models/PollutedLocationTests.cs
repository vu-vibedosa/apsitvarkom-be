using Apsitvarkom.Models;
using static Apsitvarkom.Models.Enumerations;

namespace Apsitvarkom.UnitTests.Models;

public class PollutedLocationTests
{
    [Test]
    [TestCase(-1)]
    [TestCase(101)]
    public void Constructor_InvalidProgressPassed_ThrowsArgumentOutOfRangeException(int progress) =>
        Assert.That(Assert.Throws<ArgumentOutOfRangeException>(() => new PollutedLocation { Id = Guid.NewGuid(), Progress = progress}).ParamName,
            Is.EqualTo(nameof(PollutedLocation.Progress)));

    [Test]
    [TestCase(0)]
    [TestCase(-5)]
    public void Constructor_InvalidRadiusPassed_ThrowsArgumentOutOfRangeException(int radius) =>
        Assert.That(Assert.Throws<ArgumentOutOfRangeException>(() => new PollutedLocation { Id = Guid.NewGuid(), Radius = radius }).ParamName,
            Is.EqualTo(nameof(PollutedLocation.Radius)));

    [Test]
    [TestCase((LocationSeverityLevel)0)]
    [TestCase((LocationSeverityLevel)5)]
    public void Constructor_InvalidSeverityPassed_ThrowsArgumentOutOfRangeException(LocationSeverityLevel severity) =>
        Assert.That(Assert.Throws<ArgumentOutOfRangeException>(() => new PollutedLocation { Id = Guid.NewGuid(), Severity = severity }).ParamName,
            Is.EqualTo(nameof(PollutedLocation.Severity)));

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