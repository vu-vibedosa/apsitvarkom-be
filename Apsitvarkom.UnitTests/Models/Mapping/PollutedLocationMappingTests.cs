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

    [Test]
    [TestCase("5be2354e-2500-4289-bbe2-66210592e17f",  78.948237, 35.929673, PollutedLocation.SeverityLevel.High, "notez", 4, 12, 7, PollutedLocation.SeverityLevel.Moderate, 16, "testt")]
    public void PollutedLocationUpdateRequestToPollutedLocation(string guidString, double latitude, double longitude, PollutedLocation.SeverityLevel severity, string notes, int radius, int progress, int updateRadius, PollutedLocation.SeverityLevel updateSeverity, int updateProgress, string updateNotes)
    {
        var guid = new Guid(guidString);
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
            Spotted = DateTime.UtcNow,
            Progress = progress,
            Notes = notes
        };
        
        var updateModel = new PollutedLocationUpdateRequest
        {
            Id = guid,
            Radius = updateRadius,
            Severity = updateSeverity,
            Progress = updateProgress,
            Notes = updateNotes
        };

        var mappedLocation = _mapper.Map<PollutedLocationUpdateRequest, PollutedLocation>(updateModel, businessLogicObject);

        Assert.Multiple(() =>
        {
            Assert.That(mappedLocation.Location.Coordinates.Latitude, Is.EqualTo(latitude));
            Assert.That(mappedLocation.Location.Coordinates.Longitude, Is.EqualTo(longitude));
            Assert.That(mappedLocation.Notes, Is.EqualTo(updateNotes));
            Assert.That(mappedLocation.Progress, Is.EqualTo(updateProgress));
            Assert.That(mappedLocation.Radius, Is.EqualTo(updateRadius));
            Assert.That(mappedLocation.Severity, Is.EqualTo(updateSeverity));
        });
    }

    [Test]
    [TestCase(-78.948237, 35.929673, PollutedLocation.SeverityLevel.High, "notez", 4, 12)]
    public void PollutedLocationCreateRequestToPollutedLocation(double latitude, double longitude, PollutedLocation.SeverityLevel severity, string notes, int radius, int progress)
    {
        var pollutedLocationCreateRequest = new PollutedLocationCreateRequest
        {
            Severity = severity,
            Notes = notes,
            Progress = progress,
            Radius = radius,
            Location = new LocationCreateRequest
            {
                Coordinates = new CoordinatesCreateRequest()
                {
                    Latitude = latitude,
                    Longitude = longitude
                }
            }
        };

        var pollutedLocation = _mapper.Map<PollutedLocation>(pollutedLocationCreateRequest);
        Assert.Multiple(() =>
        {
            Assert.That(pollutedLocation.Location.Coordinates.Latitude, Is.EqualTo(latitude));
            Assert.That(pollutedLocation.Location.Coordinates.Longitude, Is.EqualTo(longitude));
            Assert.That(pollutedLocation.Notes, Is.EqualTo(notes));
            Assert.That(pollutedLocation.Progress, Is.EqualTo(progress));
            Assert.That(pollutedLocation.Radius, Is.EqualTo(radius));
            Assert.That(pollutedLocation.Severity, Is.EqualTo(severity));
        });
    }
    #endregion

    #region Response mappings
    [Test]
    [TestCase("5be2354e-2500-4289-bbe2-66210592e17f", "title", -78.948237, 35.929673, 10, PollutedLocation.SeverityLevel.Low, "2022-09-16T21:43:31.0000000", 25, "Hello world")]
    public void PollutedLocationToPollutedLocationResponse(string guidString, string title, double longitude, double latitude, int radius, PollutedLocation.SeverityLevel severity, string dateTimeString, int progress, string notes)
    {
        var guid = new Guid(guidString);
        var dateTime = DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);

        var businessLogicObject = new PollutedLocation
        {
            Id = guid,
            Location =
            {
                Title = title,
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
            Assert.That(pollutedLocation.Location.Title, Is.EqualTo(title));
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