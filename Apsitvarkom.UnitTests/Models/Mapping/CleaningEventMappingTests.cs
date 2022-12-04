using Apsitvarkom.Models;
using Apsitvarkom.Models.Mapping;
using Apsitvarkom.Models.Public;
using AutoMapper;

namespace Apsitvarkom.UnitTests.Models.Mapping;

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
            PollutedLocationId = Guid.NewGuid(),
            StartTime = DateTime.Parse("2077-03-12T10:11:12Z"),
            Notes = "LL"
        };

        var mappedEvent = _mapper.Map(updateModel, businessLogicObject);

        Assert.Multiple(() =>
        {
            Assert.That(mappedEvent.Id, Is.EqualTo(businessLogicObject.Id));
            Assert.That(mappedEvent.Notes, Is.EqualTo(updateModel.Notes));
            Assert.That(mappedEvent.StartTime, Is.EqualTo(updateModel.StartTime));
            Assert.That(mappedEvent.PollutedLocationId, Is.EqualTo(updateModel.PollutedLocationId));
        });
    }
    #endregion

    #region Response mappings
    [Test]
    public void CleaningEventToCleaningEventResponse()
    {
        var businessLogicObject = new CleaningEvent
        {
            Id = Guid.NewGuid(),
            StartTime = new DateTime(2022, 9, 16, 21, 43, 31).ToUniversalTime(),
            Notes = "Hello world",
            PollutedLocationId = Guid.NewGuid()
        };

        var response = _mapper.Map<CleaningEventResponse>(businessLogicObject);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(businessLogicObject.Id));
            Assert.That(response.StartTime, Is.EqualTo(businessLogicObject.StartTime));
            Assert.That(response.Notes, Is.EqualTo(businessLogicObject.Notes));
            Assert.That(response.PollutedLocationId, Is.EqualTo(businessLogicObject.PollutedLocationId));
        });
    }
    #endregion
}