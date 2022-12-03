﻿using Apsitvarkom.Models;
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

    [Test]
    public void CleaningEventCreateRequestToCleaningEvent()
    {
        var businessLogicObject = new CleaningEvent
        {
            Id = Guid.NewGuid(),
            StartTime = new DateTime(2022, 9, 16, 21, 43, 31).ToUniversalTime(),
            Notes = "Hello world",
            PollutedLocationId = Guid.NewGuid()
        };

        var createModel = new CleaningEventCreateRequest
        {
            StartTime = new DateTime(2022, 9, 16, 21, 43, 31).ToUniversalTime(),
            Notes = "Hello world",
            PollutedLocationId = Guid.NewGuid()
        };

        var response = _mapper.Map<CleaningEventCreateRequest, CleaningEvent>(createModel, businessLogicObject);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(businessLogicObject.Id));
            Assert.That(response.StartTime, Is.EqualTo(createModel.StartTime));
            Assert.That(response.Notes, Is.EqualTo(createModel.Notes));
            Assert.That(response.PollutedLocationId, Is.EqualTo(createModel.PollutedLocationId));
        });
    }
    #endregion
}