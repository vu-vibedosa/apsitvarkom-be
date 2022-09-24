using Apsitvarkom.Models;

namespace Apsitvarkom.UnitTests.Models;

public class LocationTests
{
    [Test]
    public void Constructor_HappyPath_ValidArgumentsPassed_IsSuccess() => 
        Assert.That(
            new Location
            {
                Latitude = 12.36,
                Longitude = -143.31
            }, Is.Not.Null);
}