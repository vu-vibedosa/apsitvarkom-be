using System.Globalization;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Mapping;
using Apsitvarkom.Models.Public;
using AutoMapper;

namespace Apsitvarkom.UnitTests.Models.Mapping;

public class PollutedLocationMappingTests
{
    private IMapper _mapper = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PollutedLocationProfile>();
        });

        config.AssertConfigurationIsValid();

        _mapper = config.CreateMapper();
    }

    #region Request mappings
    [Test]
    [TestCase(-78.948237, 35.929673)]
    public void CoordinatesCreateRequestToCoordinates(double latitude, double longitude)
    {
        var coordinatesCreateRequest = new CoordinatesCreateRequest
        {
            Latitude = latitude,
            Longitude = longitude
        };

        var coordinates = _mapper.Map<Coordinates>(coordinatesCreateRequest);
        Assert.Multiple(() =>
        {
            Assert.That(coordinates.Longitude, Is.EqualTo(longitude));
            Assert.That(coordinates.Latitude, Is.EqualTo(latitude));
        });
    }
    #endregion

    #region Response mappings
    [Test]
    [TestCase("5be2354e-2500-4289-bbe2-66210592e17f", -78.948237, 35.929673, 10, PollutedLocation.SeverityLevel.Low, "2022-09-16T21:43:31.0000000", 25, "Hello world")]
    public void PollutedLocationToPollutedLocationResponse(string guidString, double longitude, double latitude, int radius, PollutedLocation.SeverityLevel severity, string dateTimeString, int progress, string notes)
    {
        var guid = new Guid(guidString);
        var dateTime = DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);

        var businessLogicObject = new PollutedLocation
        {
            Id = guid,
            Location =
            {
                Coordinates = new Coordinates
                {
                    Longitude = longitude,
                    Latitude = latitude
                },
            },
            Radius = radius,
            Severity = severity,
            Spotted = dateTime,
            Progress = progress,
            Notes = notes
        };

        var pollutedLocation = _mapper.Map<PollutedLocationResponse>(businessLogicObject);
        Assert.Multiple(() =>
        {
            Assert.That(pollutedLocation.Id, Is.EqualTo(guid));
            Assert.That(pollutedLocation.Location.Coordinates.Longitude, Is.EqualTo(longitude));
            Assert.That(pollutedLocation.Location.Coordinates.Latitude, Is.EqualTo(latitude));
            Assert.That(pollutedLocation.Radius, Is.EqualTo(radius));
            Assert.That(pollutedLocation.Severity, Is.EqualTo(severity));
            Assert.That(pollutedLocation.Spotted, Is.EqualTo(dateTime));
            Assert.That(pollutedLocation.Progress, Is.EqualTo(progress));
            Assert.That(pollutedLocation.Notes, Is.EqualTo(notes));
        });
    }
    #endregion
}