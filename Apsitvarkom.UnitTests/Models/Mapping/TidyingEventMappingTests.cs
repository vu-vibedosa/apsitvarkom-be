using Apsitvarkom.Models;
using Apsitvarkom.Models.Mapping;
using Apsitvarkom.Models.Public;
using AutoMapper;

namespace Apsitvarkom.UnitTests.Models.Mapping;

public class TidyingEventMappingTests
{
    private IMapper _mapper = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TidyingEventProfile>();
        });

        config.AssertConfigurationIsValid();

        _mapper = config.CreateMapper();
    }

    #region Request mappings

    #endregion

    #region Response mappings
    [Test]
    public void TidyingEventToTidyingEventResponse()
    {
        var businessLogicObject = new TidyingEvent
        {
            Id = Guid.NewGuid(),
            StartTime = new DateTime(2022, 9, 16, 21, 43, 31).ToUniversalTime(),
            Notes = "Hello world",
            PollutedLocationId = Guid.NewGuid()
        };

        var response = _mapper.Map<TidyingEventResponse>(businessLogicObject);
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