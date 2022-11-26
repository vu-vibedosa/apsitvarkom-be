using System.Linq.Expressions;
using Apsitvarkom.Api.Controllers;
using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Mapping;
using Apsitvarkom.Models.Public;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            Notes = "The christmas tree caught on fire."
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
        _controller = new TidyingEventController(
            _repository.Object, 
            _mapper, 
            new ObjectIdentifyRequestValidator()
        );
    }

    #region Constructor tests
    [Test]
    public void Constructor_HappyPath_IsSuccess() => Assert.That(new TidyingEventController(
            _repository.Object, 
            _mapper, 
            new ObjectIdentifyRequestValidator()
        ), Is.Not.Null);
    #endregion

    #region GetAll tests
    [Test]
    public async Task GetAll_RepositoryReturnsTidyingEvents_OKActionResultReturned()
    {
        _repository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(TidyingEvents);

        var actionResult = await _controller.GetAll();

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        Assert.That(result.Value, Is.Not.Null.And.InstanceOf<IEnumerable<TidyingEventResponse>>());
        var resultEvents = result.Value as IEnumerable<TidyingEventResponse>;
        Assert.That(resultEvents, Is.Not.Null.And.Count.EqualTo(TidyingEvents.Count()));
        for (var i = 0; i < TidyingEvents.Count(); i++)
        {
            var tidyingEvent = TidyingEvents.ElementAt(i);
            var resultEvent = resultEvents.ElementAt(i);
            Assert.Multiple(() =>
            {
                Assert.That(resultEvent.Id, Is.EqualTo(tidyingEvent.Id));
                Assert.That(resultEvent.StartTime, Is.EqualTo(tidyingEvent.StartTime));
                Assert.That(resultEvent.Notes, Is.EqualTo(tidyingEvent.Notes));
                Assert.That(resultEvent.PollutedLocationId, Is.EqualTo(tidyingEvent.PollutedLocationId));
            });
        }
    }

    [Test]
    public async Task GetAll_RepositoryThrows_Status500InternalServerErrorReturned()
    {
        _repository.Setup(r => r.GetAllAsync())
            .Throws<Exception>();

        var actionResult = await _controller.GetAll();

        Assert.That(actionResult.Result, Is.TypeOf<StatusCodeResult>());
        var result = actionResult.Result as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }
    #endregion

    #region GetById tests
    [Test]
    public async Task GetById_RepositoryReturnsTidyingEvent_OKActionResultReturned()
    {
        var tidyingEvent = TidyingEvents.First();
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = tidyingEvent.Id
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<TidyingEvent, bool>>>()))
            .ReturnsAsync(tidyingEvent);

        var actionResult = await _controller.GetById(identifyRequest);

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        Assert.That(result.Value, Is.Not.Null.And.TypeOf<TidyingEventResponse>());
        var resultEvent = result.Value as TidyingEventResponse;
        Assert.That(resultEvent, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultEvent.Id, Is.EqualTo(tidyingEvent.Id));
            Assert.That(resultEvent.StartTime, Is.EqualTo(tidyingEvent.StartTime));
            Assert.That(resultEvent.Notes, Is.EqualTo(tidyingEvent.Notes));
            Assert.That(resultEvent.PollutedLocationId, Is.EqualTo(tidyingEvent.PollutedLocationId));
        });
    }

    [Test]
    public async Task GetById_RepositoryReturnsNull_NotFoundActionResultReturned()
    {
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = Guid.NewGuid()
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<TidyingEvent, bool>>>()))
            .ReturnsAsync((TidyingEvent?)null);

        var actionResult = await _controller.GetById(identifyRequest);

        Assert.That(actionResult.Result, Is.TypeOf<NotFoundObjectResult>());
        var result = actionResult.Result as NotFoundObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task GetById_NullIdEntered_ValidationResultsInBadRequestResponseReturned()
    {
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = null
        };

        var actionResult = await _controller.GetById(identifyRequest);

        _repository.Verify(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<TidyingEvent, bool>>>()), Times.Never);

        Assert.That(actionResult.Result, Is.TypeOf<BadRequestObjectResult>());
        var result = actionResult.Result as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task GetById_RepositoryThrows_Status500InternalServerErrorReturned()
    {
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = Guid.NewGuid()
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<TidyingEvent, bool>>>()))
            .Throws<Exception>();

        var actionResult = await _controller.GetById(identifyRequest);

        Assert.That(actionResult.Result, Is.TypeOf<StatusCodeResult>());
        var result = actionResult.Result as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }
    #endregion
}