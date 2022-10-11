using System.Globalization;
using Apsitvarkom.Models;
using Apsitvarkom.Models.DTO;
using Apsitvarkom.Models.Mapping;
using AutoMapper;
using static Apsitvarkom.Models.Enumerations;

namespace Apsitvarkom.UnitTests.Models.Mapping;

public class PollutedLocationMappingTests
{
    private IMapper m_mapper;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PollutedLocationProfile>();
        });

        config.AssertConfigurationIsValid();

        m_mapper = config.CreateMapper();
    }

    [Test]
    [TestCase("5be2354e-2500-4289-bbe2-66210592e17f", -78.948237, 35.929673, 10, LocationSeverityLevel.Low, "09/16/2022 21:43:31", 25, "Hello world")]
    public void MappingToPollutedLocation_HappyPath(string guidString, double longitude, double latitude, int radius, LocationSeverityLevel severity, string dateTimeString, int progress, string notes)
    {
        var objectDTO = new PollutedLocationDTO
        {
            Id = guidString,
            Coordinates = new CoordinatesDTO
            {
                Longitude = longitude,
                Latitude = latitude
            },
            Radius = radius,
            Severity = severity.ToString(),
            Spotted = dateTimeString,
            Progress = progress,
            Notes = notes
        };

        var pollutedLocation = m_mapper.Map<PollutedLocation>(objectDTO);
        Assert.Multiple(() =>
        {
            Assert.That(pollutedLocation.Id, Is.EqualTo(Guid.Parse(guidString)));
            Assert.That(pollutedLocation.Coordinates.Longitude, Is.EqualTo(longitude));
            Assert.That(pollutedLocation.Coordinates.Latitude, Is.EqualTo(latitude));
            Assert.That(pollutedLocation.Radius, Is.EqualTo(radius));
            Assert.That(pollutedLocation.Severity, Is.EqualTo(severity));
            Assert.That(pollutedLocation.Spotted, Is.EqualTo(DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture)));
            Assert.That(pollutedLocation.Progress, Is.EqualTo(progress));
            Assert.That(pollutedLocation.Notes, Is.EqualTo(notes));
        });
    }

    [Test]
    [TestCase("5be2354e-2500-4289-bbe2-66210592e17f", -78.948237, 35.929673, 10, LocationSeverityLevel.Low, "09/16/2022 21:43:31", 25, "Hello world")]
    public void MappingToPollutedLocationDTO_HappyPath(string guidString, double longitude, double latitude, int radius, LocationSeverityLevel severity, string dateTimeString, int progress, string notes)
    {
        var guid = new Guid(guidString);
        var dateTime = DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);

        var businessLogicObject = new PollutedLocation
        {
            Id = guid,
            Coordinates = new Coordinates
            {
                Longitude = longitude,
                Latitude = latitude
            },
            Radius = radius,
            Severity = severity,
            Spotted = dateTime,
            Progress = progress,
            Notes = notes
        };

        var pollutedLocation = m_mapper.Map<PollutedLocationDTO>(businessLogicObject);
        Assert.Multiple(() =>
        {
            Assert.That(pollutedLocation.Id, Is.EqualTo(guidString));
            Assert.That(pollutedLocation.Coordinates?.Longitude, Is.EqualTo(longitude));
            Assert.That(pollutedLocation.Coordinates?.Latitude, Is.EqualTo(latitude));
            Assert.That(pollutedLocation.Radius, Is.EqualTo(radius));
            Assert.That(pollutedLocation.Severity, Is.EqualTo(severity.ToString()));
            Assert.That(pollutedLocation.Spotted, Is.EqualTo(dateTimeString));
            Assert.That(pollutedLocation.Progress, Is.EqualTo(progress));
            Assert.That(pollutedLocation.Notes, Is.EqualTo(notes));
        });
    }
}