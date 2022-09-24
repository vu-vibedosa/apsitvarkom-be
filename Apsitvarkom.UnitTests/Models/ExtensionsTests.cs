using Apsitvarkom.Models;

namespace Apsitvarkom.UnitTests.Models;

public class ExtensionsTests
{
    #region DistanceTo tests
    [Test]
    public void DistanceTo_LocationComparedToItself_ReturnsZero()
    {
        var location = new Location
        {
            Latitude = 41.12121,
            Longitude = -31.1234
        };
        Assert.That(location.DistanceTo(location), Is.Zero);
    }

    [Test]
    public void DistanceTo_DistanceSourceAndDestinationExchanged_ReturnsEqualDistances()
    {
        var location1 = new Location
        {
            Latitude = 41.12121,
            Longitude = -31.1234
        };
        var location2 = new Location
        {
            Latitude = -87.12121,
            Longitude = 1.14234
        };
        Assert.That(location1.DistanceTo(location2), Is.EqualTo(location2.DistanceTo(location1)));
    }

    [Test]
    [TestCase(54.6872, 25.2797, 54.687866, 25.275202, 298.4, 0.8952)]
    [TestCase(54.730026, 25.262100, 54.675209, 25.273415, 6139, 18.417)]
    public void DistanceTo_ReturnsAccurateDistanceBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2, double expected, double delta)
    {
        // expected values are taken from https://www.movable-type.co.uk/scripts/latlong.html calculator, delta - this tool's estimated 0.3% inaccuracy rate
        var location1 = new Location
        {
            Latitude = latitude1,
            Longitude = longitude1
        };
        var location2 = new Location
        {
            Latitude = latitude2,
            Longitude = longitude2
        };
        var actual = location1.DistanceTo(location2);
        Assert.That(Math.Abs(actual - expected), Is.LessThan(delta));
    }
    #endregion
}