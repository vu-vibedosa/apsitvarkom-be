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
            Id = Guid.NewGuid(),
            PollutedLocationId = Guid.NewGuid(),
            StartTime = DateTime.Parse("2022-12-11T10:11:12Z"),
        },
        new()
        {
            Id = Guid.NewGuid(),
            PollutedLocationId = Guid.NewGuid(),
            StartTime = DateTime.Parse("2023-01-01T00:11:22Z"),
            Notes = "So many fireworks leftovers..."
        },
        new()
        {
            Id = Guid.NewGuid(),
            PollutedLocationId = Guid.NewGuid(),
            StartTime = DateTime.Parse("2022-12-23T10:11:12Z"),
            Notes = "The christmas tree caught on fire.."
        },
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