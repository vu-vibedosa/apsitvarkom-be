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
    public void CoordinatesCreateRequestToCoordinates()
    {
        var coordinatesCreateRequest = new CoordinatesCreateRequest
        {
            Latitude = -78.948237,
            Longitude = 35.929673
        };

        var coordinates = _mapper.Map<Coordinates>(coordinatesCreateRequest);
        Assert.Multiple(() =>
        {
            Assert.That(coordinates.Longitude, Is.EqualTo(coordinatesCreateRequest.Longitude));
            Assert.That(coordinates.Latitude, Is.EqualTo(coordinatesCreateRequest.Latitude));
        });
    }

    [Test]
    public void PollutedLocationCreateRequestToPollutedLocation()
    {
        var pollutedLocationCreateRequest = new PollutedLocationCreateRequest
        {
            Severity = PollutedLocation.SeverityLevel.High,
            Notes = "notez",
            Radius = 4,
            Location = new LocationCreateRequest
            {
                Coordinates = new CoordinatesCreateRequest()
                {
                    Latitude = -78.948237,
                    Longitude = 35.929673
                }
            }
        };

        var pollutedLocation = _mapper.Map<PollutedLocation>(pollutedLocationCreateRequest);
        Assert.Multiple(() =>
        {
            Assert.That(pollutedLocation.Location.Coordinates.Latitude, Is.EqualTo(pollutedLocationCreateRequest.Location.Coordinates.Latitude));
            Assert.That(pollutedLocation.Location.Coordinates.Longitude, Is.EqualTo(pollutedLocationCreateRequest.Location.Coordinates.Longitude));
            Assert.That(pollutedLocation.Notes, Is.EqualTo(pollutedLocationCreateRequest.Notes));
            Assert.That(pollutedLocation.Radius, Is.EqualTo(pollutedLocationCreateRequest.Radius));
            Assert.That(pollutedLocation.Severity, Is.EqualTo(pollutedLocationCreateRequest.Severity));
        });
    }
    #endregion

    #region Response mappings
    [Test]
    public void PollutedLocationToPollutedLocationResponse()
    {
        var pollutedLocationId = Guid.NewGuid();
        var spotted = new DateTime(2022, 9, 16, 21, 43, 31).ToUniversalTime();
        var eventId1 = Guid.NewGuid();
        var eventStartTime1 = new DateTime(2022, 10, 11, 12, 13, 14).ToUniversalTime();
        var eventId2 = Guid.NewGuid();
        var eventStartTime2 = new DateTime(2022, 11, 12, 13, 14, 15).ToUniversalTime();

        var businessLogicObject = new PollutedLocation
        {
            Id = pollutedLocationId,
            Location =
            {
                Title = "title",
                Coordinates = new Coordinates
                {
                    Longitude = -78.948237,
                    Latitude = 35.929673
                },
            },
            Radius = 10,
            Severity = PollutedLocation.SeverityLevel.Low,
            Spotted = spotted,
            Progress = 25,
            Notes = "Hello world",
            Events = new List<TidyingEvent>()
            {
                new()
                {
                    PollutedLocationId = pollutedLocationId,
                    Id = eventId1,
                    Notes = "Hello NPC",
                    StartTime = eventStartTime1,
                },
                new()
                {
                    PollutedLocationId = pollutedLocationId,
                    Id = eventId2,
                    StartTime = eventStartTime2,
                }
            }
        };

        var pollutedLocation = _mapper.Map<PollutedLocationResponse>(businessLogicObject);
        Assert.Multiple(() =>
        {
            Assert.That(pollutedLocation.Id, Is.EqualTo(businessLogicObject.Id));
            Assert.That(pollutedLocation.Location.Title, Is.EqualTo(businessLogicObject.Location.Title));
            Assert.That(pollutedLocation.Location.Coordinates.Longitude, Is.EqualTo(businessLogicObject.Location.Coordinates.Longitude));
            Assert.That(pollutedLocation.Location.Coordinates.Latitude, Is.EqualTo(businessLogicObject.Location.Coordinates.Latitude));
            Assert.That(pollutedLocation.Radius, Is.EqualTo(businessLogicObject.Radius));
            Assert.That(pollutedLocation.Severity, Is.EqualTo(businessLogicObject.Severity));
            Assert.That(pollutedLocation.Spotted, Is.EqualTo(businessLogicObject.Spotted));
            Assert.That(pollutedLocation.Progress, Is.EqualTo(businessLogicObject.Progress));
            Assert.That(pollutedLocation.Notes, Is.EqualTo(businessLogicObject.Notes));
        });
    }
    #endregion
}