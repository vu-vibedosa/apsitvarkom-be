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
    [TestCase(-78.948237, 35.929673, PollutedLocation.SeverityLevel.High, "notez", 4)]
    public void PollutedLocationCreateRequestToPollutedLocation(double latitude, double longitude, PollutedLocation.SeverityLevel severity, string notes, int radius)
    {
        var pollutedLocationCreateRequest = new PollutedLocationCreateRequest
        {
            Severity = severity,
            Notes = notes,
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
            Assert.That(pollutedLocation.Radius, Is.EqualTo(radius));
            Assert.That(pollutedLocation.Severity, Is.EqualTo(severity));
        });
    }

    [Test]
    public void PollutedLocationUpdateRequestToPollutedLocation_AllPropertiesNotNull()
    {
        var businessLogicObject = new PollutedLocation
        {
            Id = Guid.NewGuid(),
            Location = 
            {
                Title = "title",
                Coordinates = new Coordinates
                {
                    Longitude = 35.929673,
                    Latitude = 78.948237
                },
            },
            Radius = 4,
            Severity = PollutedLocation.SeverityLevel.High,
            Spotted = DateTime.UtcNow,
            Progress = 12,
            Notes = "notez"
        };

        var updateModel = new PollutedLocationUpdateRequest
        {
            Id = businessLogicObject.Id,
            Radius = 7,
            Severity = PollutedLocation.SeverityLevel.Moderate,
            Progress = 16,
            Notes = "testt"
        };

        var mappedLocation = _mapper.Map<PollutedLocationUpdateRequest, PollutedLocation>(updateModel, businessLogicObject);

        Assert.Multiple(() =>
        {
            Assert.That(mappedLocation.Location.Coordinates.Latitude, Is.EqualTo(businessLogicObject.Location.Coordinates.Latitude));
            Assert.That(mappedLocation.Location.Coordinates.Longitude, Is.EqualTo(businessLogicObject.Location.Coordinates.Longitude));
            Assert.That(mappedLocation.Location.Title, Is.EqualTo(businessLogicObject.Location.Title));
            Assert.That(mappedLocation.Spotted, Is.EqualTo(businessLogicObject.Spotted));
            Assert.That(mappedLocation.Id, Is.EqualTo(businessLogicObject.Id));
            Assert.That(mappedLocation.Notes, Is.EqualTo(updateModel.Notes));
            Assert.That(mappedLocation.Progress, Is.EqualTo(updateModel.Progress));
            Assert.That(mappedLocation.Radius, Is.EqualTo(updateModel.Radius));
            Assert.That(mappedLocation.Severity, Is.EqualTo(updateModel.Severity));
        });
    }

    [Test]
    public void PollutedLocationUpdateRequestToPollutedLocation_SomePropertiesNull_KeepsBusinessObjectValues()
    {
        var businessLogicObject = new PollutedLocation
        {
            Id = Guid.NewGuid(),
            Location =
            {
                Title = "title",
                Coordinates = new Coordinates
                {
                    Longitude = 35.929673,
                    Latitude = 78.948237
                },
            },
            Radius = 4,
            Severity = PollutedLocation.SeverityLevel.High,
            Spotted = DateTime.UtcNow,
            Progress = 12,
            Notes = "notez"
        };

        var updateModel = new PollutedLocationUpdateRequest
        {
            Id = businessLogicObject.Id,
            Radius = 7,
            Severity = PollutedLocation.SeverityLevel.Moderate,
        };

        var mappedLocation = _mapper.Map<PollutedLocationUpdateRequest, PollutedLocation>(updateModel, businessLogicObject);

        Assert.Multiple(() =>
        {
            Assert.That(mappedLocation.Location.Coordinates.Latitude, Is.EqualTo(businessLogicObject.Location.Coordinates.Latitude));
            Assert.That(mappedLocation.Location.Coordinates.Longitude, Is.EqualTo(businessLogicObject.Location.Coordinates.Longitude));
            Assert.That(mappedLocation.Location.Title, Is.EqualTo(businessLogicObject.Location.Title));
            Assert.That(mappedLocation.Spotted, Is.EqualTo(businessLogicObject.Spotted));
            Assert.That(mappedLocation.Id, Is.EqualTo(businessLogicObject.Id));
            Assert.That(mappedLocation.Notes, Is.EqualTo(businessLogicObject.Notes));
            Assert.That(mappedLocation.Progress, Is.EqualTo(businessLogicObject.Progress));
            Assert.That(mappedLocation.Radius, Is.EqualTo(updateModel.Radius));
            Assert.That(mappedLocation.Severity, Is.EqualTo(updateModel.Severity));
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
}