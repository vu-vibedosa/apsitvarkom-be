using Apsitvarkom.Models;

namespace Apsitvarkom.UnitTests.Models;

public class LocationTests
{
    [Test]
    [TestCase(-91.1, 14.7, "Latitude")]
    [TestCase(43.9, 184.4, "Longitude")]
    [TestCase(98.12, -193.53, "Latitude")]
    public void Constructor_InvalidArgumentsPassed_ThrowsArgumentOutOfRangeException(double latitude, double longitude, string failedParameterName) =>
        Assert.That(Assert.Throws<ArgumentOutOfRangeException>(() => new Location { Latitude = latitude, Longitude = longitude }).ParamName, 
            Is.EqualTo(failedParameterName));

    [Test]
    public void Constructor_HappyPath_ValidArgumentsPassed_IsSuccess() => 
        Assert.That(
            new Location
            {
                Latitude = 12.36,
                Longitude = -143.31
            }, Is.Not.Null);
}