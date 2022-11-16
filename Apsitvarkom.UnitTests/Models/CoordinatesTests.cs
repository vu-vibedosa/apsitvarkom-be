using Apsitvarkom.Models;

namespace Apsitvarkom.UnitTests.Models;

public class CoordinatesTests
{
    #region DistanceTo tests
    [Test]
    public void DistanceTo_LocationComparedToItself_ReturnsZero()
    {
        var coordinates = new Coordinates
        {
            Latitude = 41.12121,
            Longitude = -31.1234
        };
        Assert.That(coordinates.DistanceTo(coordinates), Is.Zero);
    }

    [Test]
    public void DistanceTo_DistanceSourceAndDestinationExchanged_ReturnsEqualDistances()
    {
        var coordinates1 = new Coordinates
        {
            Latitude = 41.12121,
            Longitude = -31.1234
        };
        var coordinates2 = new Coordinates
        {
            Latitude = -87.12121,
            Longitude = 1.14234
        };
        Assert.That(coordinates1.DistanceTo(coordinates2), Is.EqualTo(coordinates2.DistanceTo(coordinates1)));
    }

    [Test]
    [TestCase(54.6872, 25.2797, 54.687866, 25.275202, 298.4, 0.8952)]
    [TestCase(54.730026, 25.262100, 54.675209, 25.273415, 6139, 18.417)]
    public void DistanceTo_ReturnsAccurateDistanceBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2, double expected, double delta)
    {
        // expected values are taken from https://www.movable-type.co.uk/scripts/latlong.html calculator, delta - this tool's estimated 0.3% inaccuracy rate
        var coordinates1 = new Coordinates
        {
            Latitude = latitude1,
            Longitude = longitude1
        };
        var coordinates2 = new Coordinates
        {
            Latitude = latitude2,
            Longitude = longitude2
        };
        var actual = coordinates1.DistanceTo(coordinates2);
        Assert.That(Math.Abs(actual - expected), Is.LessThan(delta));
    }
    #endregion
}