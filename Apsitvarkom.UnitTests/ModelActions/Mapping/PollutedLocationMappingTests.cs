using Apsitvarkom.ModelActions.Mapping;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using AutoMapper;

namespace Apsitvarkom.UnitTests.ModelActions.Mapping;

public class PollutedLocationMappingTests
{
    private IMapper _mapper = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PollutedLocationProfile>();
            cfg.AddProfile<CleaningEventProfile>();
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
                Coordinates = new CoordinatesCreateRequest
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

    [Test]
    public void ObjectIdentifyRequestToPollutedLocation()
    {
        var objectIdentifyRequest = new ObjectIdentifyRequest
        {
            Id = Guid.NewGuid()
        };

        var location = _mapper.Map<PollutedLocation>(objectIdentifyRequest);

        Assert.That(location.Id, Is.EqualTo(objectIdentifyRequest.Id));
    }

    [Test]
    public void PollutedLocationUpdateRequestToPollutedLocation_AllPropertiesNotNull()
    {
        var businessLogicObject = new PollutedLocation
        {
            Id = Guid.NewGuid(),
            Location = 
            {
                Title = new("title", "pavadinimas"),
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
            Notes = "testt"
        };

        var mappedLocation = _mapper.Map<PollutedLocationUpdateRequest, PollutedLocation>(updateModel, businessLogicObject);

        Assert.Multiple(() =>
        {
            Assert.That(mappedLocation.Location.Coordinates.Latitude, Is.EqualTo(businessLogicObject.Location.Coordinates.Latitude));
            Assert.That(mappedLocation.Location.Coordinates.Longitude, Is.EqualTo(businessLogicObject.Location.Coordinates.Longitude));
            Assert.That(mappedLocation.Location.Title.English, Is.EqualTo(businessLogicObject.Location.Title.English));
            Assert.That(mappedLocation.Location.Title.Lithuanian, Is.EqualTo(businessLogicObject.Location.Title.Lithuanian));
            Assert.That(mappedLocation.Spotted, Is.EqualTo(businessLogicObject.Spotted));
            Assert.That(mappedLocation.Id, Is.EqualTo(businessLogicObject.Id));
            Assert.That(mappedLocation.Notes, Is.EqualTo(updateModel.Notes));
            Assert.That(mappedLocation.Progress, Is.EqualTo(businessLogicObject.Progress));
            Assert.That(mappedLocation.Radius, Is.EqualTo(updateModel.Radius));
            Assert.That(mappedLocation.Severity, Is.EqualTo(updateModel.Severity));
        });
    }

    [Test]
    public void PollutedLocationUpdateRequestToPollutedLocation_NotesNull_NotesSetToNull()
    {
        var businessLogicObject = new PollutedLocation
        {
            Id = Guid.NewGuid(),
            Location =
            {
                Title = new("title", "pavadinimas"),
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
            Assert.That(mappedLocation.Location.Title.English, Is.EqualTo(businessLogicObject.Location.Title.English));
            Assert.That(mappedLocation.Location.Title.Lithuanian, Is.EqualTo(businessLogicObject.Location.Title.Lithuanian));
            Assert.That(mappedLocation.Spotted, Is.EqualTo(businessLogicObject.Spotted));
            Assert.That(mappedLocation.Id, Is.EqualTo(businessLogicObject.Id));
            Assert.That(mappedLocation.Notes, Is.Null);
            Assert.That(mappedLocation.Progress, Is.EqualTo(businessLogicObject.Progress));
            Assert.That(mappedLocation.Radius, Is.EqualTo(updateModel.Radius));
            Assert.That(mappedLocation.Severity, Is.EqualTo(updateModel.Severity));
        });
    }
    #endregion

    #region Response mappings
    [Test]
    public void PollutedLocationToPollutedLocationResponse()
    {
        var pollutedLocationId = Guid.NewGuid();

        var businessLogicObject = new PollutedLocation
        {
            Id = pollutedLocationId,
            Location =
            {
                Title = new("title", "pavadinimas"),
                Coordinates = new Coordinates
                {
                    Longitude = -78.948237,
                    Latitude = 35.929673
                }
            },
            Radius = 10,
            Severity = PollutedLocation.SeverityLevel.Low,
            Spotted = new DateTime(2022, 9, 16, 21, 43, 31).ToUniversalTime(),
            Progress = 25,
            Notes = "Hello world",
            Events = new List<CleaningEvent>
            {
                new()
                {
                    PollutedLocationId = pollutedLocationId,
                    Id = Guid.NewGuid(),
                    Notes = "Hello NPC",
                    IsFinalized = true,
                    StartTime = new DateTime(2022, 10, 11, 12, 13, 14).ToUniversalTime(),
                },
                new()
                {
                    PollutedLocationId = pollutedLocationId,
                    Id = Guid.NewGuid(),
                    IsFinalized = true,
                    StartTime = new DateTime(2022, 11, 12, 13, 14, 15).ToUniversalTime(),
                }
            }
        };

        var pollutedLocation = _mapper.Map<PollutedLocationResponse>(businessLogicObject);

        Assert.Multiple(() =>
        {
            Assert.That(pollutedLocation.Id, Is.EqualTo(businessLogicObject.Id));
            Assert.That(pollutedLocation.Location.Title.En, Is.EqualTo(businessLogicObject.Location.Title.English));
            Assert.That(pollutedLocation.Location.Title.Lt, Is.EqualTo(businessLogicObject.Location.Title.Lithuanian));
            Assert.That(pollutedLocation.Location.Coordinates.Longitude, Is.EqualTo(businessLogicObject.Location.Coordinates.Longitude));
            Assert.That(pollutedLocation.Location.Coordinates.Latitude, Is.EqualTo(businessLogicObject.Location.Coordinates.Latitude));
            Assert.That(pollutedLocation.Radius, Is.EqualTo(businessLogicObject.Radius));
            Assert.That(pollutedLocation.Severity, Is.EqualTo(businessLogicObject.Severity));
            Assert.That(pollutedLocation.Spotted, Is.EqualTo(businessLogicObject.Spotted));
            Assert.That(pollutedLocation.Progress, Is.EqualTo(businessLogicObject.Progress));
            Assert.That(pollutedLocation.Notes, Is.EqualTo(businessLogicObject.Notes));
            Assert.That(pollutedLocation.Events, Has.Count.EqualTo(businessLogicObject.Events.Count));
            for (var j = 0; j < pollutedLocation.Events.Count; ++j)
            {
                Assert.That(pollutedLocation.Events[j].PollutedLocationId, Is.EqualTo(businessLogicObject.Events[j].PollutedLocationId));
                Assert.That(pollutedLocation.Events[j].Id, Is.EqualTo(businessLogicObject.Events[j].Id));
                Assert.That(pollutedLocation.Events[j].Notes, Is.EqualTo(businessLogicObject.Events[j].Notes));
                Assert.That(pollutedLocation.Events[j].StartTime, Is.EqualTo(businessLogicObject.Events[j].StartTime));
                Assert.That(pollutedLocation.Events[j].Status, Is.EqualTo(CleaningEventResponse.State.Finalized));
            }
        });
    }
    #endregion
}