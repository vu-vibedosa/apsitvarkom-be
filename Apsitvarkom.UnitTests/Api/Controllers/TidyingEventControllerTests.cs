using Apsitvarkom.Api.Controllers;
using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Mapping;
using AutoMapper;
using Moq;

namespace Apsitvarkom.UnitTests.Api.Controllers;

public class TidyingEventControllerTests
{
    private TidyingEventController _controller;
    private IMapper _mapper;
    private Mock<IRepository<TidyingEvent>> _repository;

    private readonly IEnumerable<TidyingEvent> TidyingEvents = new List<TidyingEvent>
    {
        new()
        {
            Id = Guid.Parse("3408f21c-90d3-470f-afe2-0b6f51c8e405"),
            PollutedLocationId = Guid.Parse("6f61b9b1-8aea-4b95-ba49-32af5e96fb9c"),
            StartTime = DateTime.Parse("2022-12-11T10:11:12Z")
        },
        new()
        {
            Id = Guid.Parse("2482d7a9-e64c-4159-a570-f51d500f0806"),
            PollutedLocationId = Guid.Parse("548d8c82-2822-482c-a801-76916d4b770d"),
            StartTime = DateTime.Parse("2023-01-01T00:11:22Z"),
            Notes = "So many fireworks leftovers..."
        },
        new()
        {
            Id = Guid.Parse("6e18987e-497b-4a35-820f-283321c9a9dd"),
            PollutedLocationId = Guid.Parse("6f61b9b1-8aea-4b95-ba49-32af5e96fb9c"),
            StartTime = DateTime.Parse("2022-12-23T10:11:12Z"),
            Notes = "The christmas tree caught on fire.."
        }
    };

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TidyingEventProfile>();
        });
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();

        _repository = new Mock<IRepository<TidyingEvent>>();
        _controller = new TidyingEventController(_repository.Object, _mapper);
    }

    #region Constructor tests
    [Test]
    public void Constructor_HappyPath_IsSuccess() => Assert.That(new TidyingEventController(_repository.Object, _mapper), Is.Not.Null);
    #endregion
}