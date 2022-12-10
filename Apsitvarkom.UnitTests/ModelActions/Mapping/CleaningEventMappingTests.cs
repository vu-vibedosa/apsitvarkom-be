using Apsitvarkom.ModelActions.Mapping;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using AutoMapper;

namespace Apsitvarkom.UnitTests.ModelActions.Mapping;

public class CleaningEventMappingTests
{
    private IMapper _mapper = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CleaningEventProfile>();
        });

        config.AssertConfigurationIsValid();

        _mapper = config.CreateMapper();
    }

    #region Request mappings
    [Test]
    public void ObjectIdentifyRequestToCleaningEvent()
    {
        var request = new ObjectIdentifyRequest
        {
            Id = Guid.NewGuid()
        };

        var response = _mapper.Map<CleaningEvent>(request);

        Assert.That(response.Id, Is.EqualTo(request.Id));
    }

    [Test]
    public void CleaningEventCreateRequestToCleaningEvent()
    {
        var createModel = new CleaningEventCreateRequest
        {
            StartTime = new DateTime(2022, 9, 16, 21, 43, 31).ToUniversalTime(),
            Notes = "Hello world",
            PollutedLocationId = Guid.NewGuid()
        };

        var response = _mapper.Map<CleaningEvent>(createModel);

        Assert.Multiple(() =>
        {
            Assert.That(response.StartTime, Is.EqualTo(createModel.StartTime));
            Assert.That(response.Notes, Is.EqualTo(createModel.Notes));
            Assert.That(response.PollutedLocationId, Is.EqualTo(createModel.PollutedLocationId));
        });
    }

    [Test]
    public void CleaningEventUpdateRequestToPollutedLocation_AllPropertiesNotNull()
    {
        var businessLogicObject = new CleaningEvent
        {
            Id = Guid.NewGuid(),
            PollutedLocationId = Guid.NewGuid(),
            StartTime = DateTime.Parse("2077-05-12T10:11:12Z"),
            Notes = "L"
        };

        var updateModel = new CleaningEventUpdateRequest
        {
            Id = businessLogicObject.Id,
            StartTime = DateTime.Parse("2077-03-12T10:11:12Z"),
            Notes = "LL"
        };

        var mappedEvent = _mapper.Map(updateModel, businessLogicObject);

        Assert.Multiple(() =>
        {
            Assert.That(mappedEvent.Id, Is.EqualTo(businessLogicObject.Id));
            Assert.That(mappedEvent.Notes, Is.EqualTo(updateModel.Notes));
            Assert.That(mappedEvent.StartTime, Is.EqualTo(updateModel.StartTime));
        });
    }

    [Test]
    public void CleaningEventUpdateRequestToPollutedLocation_NotesNull_NotesSetToNull()
    {
        var businessLogicObject = new CleaningEvent
        {
            Id = Guid.NewGuid(),
            PollutedLocationId = Guid.NewGuid(),
            StartTime = DateTime.Parse("2077-05-12T10:11:12Z"),
            Notes = "L"
        };

        var updateModel = new CleaningEventUpdateRequest
        {
            Id = businessLogicObject.Id,
            StartTime = DateTime.Parse("2077-03-12T10:11:12Z"),
        };

        var mappedEvent = _mapper.Map(updateModel, businessLogicObject);

        Assert.Multiple(() =>
        {
            Assert.That(mappedEvent.Id, Is.EqualTo(businessLogicObject.Id));
            Assert.That(mappedEvent.Notes, Is.Null);
            Assert.That(mappedEvent.StartTime, Is.EqualTo(updateModel.StartTime));
        });
    }
    #endregion

    #region Response mappings
    [Test]
    [TestCase(true, 0, CleaningEventResponse.State.Finalized)]
    [TestCase(false, 1, CleaningEventResponse.State.Foreseen)]
    [TestCase(false, -1, CleaningEventResponse.State.Finished)]
    public void CleaningEventToCleaningEventResponse(bool isFinalized, int daysFromCurrentDate, CleaningEventResponse.State result)
    {
        var businessLogicObject = new CleaningEvent
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(daysFromCurrentDate).ToUniversalTime(),
            Notes = "Hello world",
            PollutedLocationId = Guid.NewGuid(),
            IsFinalized = isFinalized
        };

        var response = _mapper.Map<CleaningEventResponse>(businessLogicObject);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(businessLogicObject.Id));
            Assert.That(response.StartTime, Is.EqualTo(businessLogicObject.StartTime));
            Assert.That(response.Notes, Is.EqualTo(businessLogicObject.Notes));
            Assert.That(response.PollutedLocationId, Is.EqualTo(businessLogicObject.PollutedLocationId));
            Assert.That(response.Status, Is.EqualTo(result));
        });
    }
    #endregion
}