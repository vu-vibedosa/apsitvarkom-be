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

public class CleaningEventControllerTests
{
    private CleaningEventController _controller;
    private IMapper _mapper;
    private Mock<IRepository<CleaningEvent>> _repository;

    private readonly IEnumerable<CleaningEvent> CleaningEvents = new List<CleaningEvent>
    {
        new()
        {
            Id = Guid.Parse("3408f21c-90d3-470f-afe2-0b6f51c8e405"),
            PollutedLocationId = Guid.Parse("6f61b9b1-8aea-4b95-ba49-32af5e96fb9c"),
            StartTime = DateTime.Parse("2026-12-11T10:11:12Z")
        },
        new()
        {
            Id = Guid.Parse("2482d7a9-e64c-4159-a570-f51d500f0806"),
            PollutedLocationId = Guid.Parse("548d8c82-2822-482c-a801-76916d4b770d"),
            StartTime = DateTime.Parse("2032-01-01T00:11:22Z"),
            Notes = "So many fireworks leftovers..."
        },
        new()
        {
            Id = Guid.Parse("6e18987e-497b-4a35-820f-283321c9a9dd"),
            PollutedLocationId = Guid.Parse("6f61b9b1-8aea-4b95-ba49-32af5e96fb9c"),
            StartTime = DateTime.Parse("2052-12-23T10:11:12Z"),
            Notes = "The christmas tree caught on fire."
        }
    };

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CleaningEventProfile>();
        });
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();

        _repository = new Mock<IRepository<CleaningEvent>>();
        _controller = new CleaningEventController(
            _repository.Object,
            _mapper, 
            new ObjectIdentifyRequestValidator(),
            new CleaningEventCreateRequestValidator(),
            new CleaningEventUpdateRequestValidator()
        );
    }

    #region Constructor tests
    [Test]
    public void Constructor_HappyPath_IsSuccess() => Assert.That(new CleaningEventController(
            _repository.Object,
            _mapper, 
            new ObjectIdentifyRequestValidator(),
            new CleaningEventCreateRequestValidator(),
            new CleaningEventUpdateRequestValidator()
        ), Is.Not.Null);
    #endregion

    #region GetAll tests
    [Test]
    public async Task GetAll_RepositoryReturnsCleaningEvents_OKActionResultReturned()
    {
        _repository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(CleaningEvents);

        var actionResult = await _controller.GetAll();

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        Assert.That(result.Value, Is.Not.Null.And.InstanceOf<IEnumerable<CleaningEventResponse>>());
        var resultEvents = result.Value as IEnumerable<CleaningEventResponse>;
        Assert.That(resultEvents, Is.Not.Null.And.Count.EqualTo(CleaningEvents.Count()));

        var sortedRepositoryEvents = CleaningEvents.OrderBy(o => o.StartTime);

        for (var i = 0; i < CleaningEvents.Count(); i++)
        {
            var cleaningEvent = sortedRepositoryEvents.ElementAt(i);
            var resultEvent = resultEvents.ElementAt(i);
            Assert.Multiple(() =>
            {
                Assert.That(resultEvent.Id, Is.EqualTo(cleaningEvent.Id));
                Assert.That(resultEvent.StartTime, Is.EqualTo(cleaningEvent.StartTime));
                Assert.That(resultEvent.Notes, Is.EqualTo(cleaningEvent.Notes));
                Assert.That(resultEvent.PollutedLocationId, Is.EqualTo(cleaningEvent.PollutedLocationId));
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
    public async Task GetById_RepositoryReturnsCleaningEvent_OKActionResultReturned()
    {
        var cleaningEvent = CleaningEvents.First();
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = cleaningEvent.Id
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()))
            .ReturnsAsync(cleaningEvent);

        var actionResult = await _controller.GetById(identifyRequest);

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        Assert.That(result.Value, Is.Not.Null.And.TypeOf<CleaningEventResponse>());
        var resultEvent = result.Value as CleaningEventResponse;
        Assert.That(resultEvent, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultEvent.Id, Is.EqualTo(cleaningEvent.Id));
            Assert.That(resultEvent.StartTime, Is.EqualTo(cleaningEvent.StartTime));
            Assert.That(resultEvent.Notes, Is.EqualTo(cleaningEvent.Notes));
            Assert.That(resultEvent.PollutedLocationId, Is.EqualTo(cleaningEvent.PollutedLocationId));
        });
    }

    [Test]
    public async Task GetById_RepositoryReturnsNull_NotFoundActionResultReturned()
    {
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = Guid.NewGuid()
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()))
            .ReturnsAsync((CleaningEvent?)null);

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

        _repository.Verify(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()), Times.Never);

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

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()))
            .Throws<Exception>();

        var actionResult = await _controller.GetById(identifyRequest);

        Assert.That(actionResult.Result, Is.TypeOf<StatusCodeResult>());
        var result = actionResult.Result as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }
    #endregion

    #region Create tests
    [Test]
    public async Task Create_RepositoryInsertsOnce_CreatedAtActionResultReturned()
    {
        var cleaningEvent = CleaningEvents.First();
        var createRequest = new CleaningEventCreateRequest
        {
            PollutedLocationId = Guid.NewGuid(),
            StartTime = cleaningEvent.StartTime,
            Notes = cleaningEvent.Notes
        };

        var actionResult = await _controller.Create(createRequest);

        _repository.Verify(r => r.InsertAsync(It.IsAny<CleaningEvent>()), Times.Once);

        Assert.That(actionResult.Result, Is.TypeOf<CreatedAtActionResult>());
        var result = actionResult.Result as CreatedAtActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));

        Assert.That(result.Value, Is.Not.Null.And.TypeOf<CleaningEventResponse>());
        var resultLocation = result.Value as CleaningEventResponse;
        Assert.That(resultLocation, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultLocation.PollutedLocationId, expression: Is.EqualTo(createRequest.PollutedLocationId));
            Assert.That(resultLocation.StartTime, Is.EqualTo(createRequest.StartTime));
            Assert.That(resultLocation.Notes, Is.EqualTo(createRequest.Notes));
        });
    }

    [Test]
    public async Task Create_RepositoryThrows_Status500InternalServerErrorReturned()
    {
        var cleaningEvent = CleaningEvents.First();
        var createRequest = new CleaningEventCreateRequest
        {
            PollutedLocationId = Guid.NewGuid(),
            StartTime = cleaningEvent.StartTime,
            Notes = cleaningEvent.Notes
        };

        _repository.Setup(r => r.InsertAsync(It.IsAny<CleaningEvent>())).Throws<Exception>();

        var actionResult = await _controller.Create(createRequest);

        Assert.That(actionResult.Result, Is.TypeOf<StatusCodeResult>());
        var result = actionResult.Result as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task Create_InvalidDataEntered_ValidationResultsInBadRequestResponseReturned()
    {
        var createRequest = new CleaningEventCreateRequest
        {
            PollutedLocationId = Guid.NewGuid(),
            StartTime = null
        };

        var actionResult = await _controller.Create(createRequest);

        _repository.Verify(r => r.InsertAsync(It.IsAny<CleaningEvent>()), Times.Never);

        Assert.That(actionResult.Result, Is.TypeOf<BadRequestObjectResult>());
        var result = actionResult.Result as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }
    #endregion

    #region Update tests
    [Test]
    public async Task Update_NullIdEntered_ValidationResultsInBadRequestResponseReturned()
    {
        var updateRequest = new CleaningEventUpdateRequest
        {
            Id = null
        };

        var actionResult = await _controller.Update(updateRequest);

        _repository.Verify(r => r.UpdateAsync(It.IsAny<CleaningEvent>()), Times.Never);

        Assert.That(actionResult.Result, Is.TypeOf<BadRequestObjectResult>());
        var result = actionResult.Result as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Update_RepositoryThrowsAcquiringCleaningEvent_Status500InternalServerErrorReturned()
    {
        var updateRequest = new CleaningEventUpdateRequest
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>())).Throws<Exception>();

        var actionResult = await _controller.Update(updateRequest);

        _repository.Verify(r => r.UpdateAsync(It.IsAny<CleaningEvent>()), Times.Never);

        Assert.That(actionResult.Result, Is.TypeOf<StatusCodeResult>());
        var result = actionResult.Result as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task Update_RepositoryReturnsNullAcquiringInstance_NotFoundActionResultReturned()
    {
        var updateRequest = new CleaningEventUpdateRequest
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            Notes = "boop"
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>())).ReturnsAsync((CleaningEvent?)null);

        var actionResult = await _controller.Update(updateRequest);

        _repository.Verify(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()), Times.Once);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<CleaningEvent>()), Times.Never);

        Assert.That(actionResult.Result, Is.TypeOf<NotFoundObjectResult>());
        var result = actionResult.Result as NotFoundObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Update_RepositoryThrowsUpdatingCleaningEvent_Status500InternalServerErrorReturned()
    {
        var updateRequest = new CleaningEventUpdateRequest
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.Parse("2032-01-01T00:11:22Z"),
            Notes = "Updated notes"
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>())).ReturnsAsync(CleaningEvents.First());
        _repository.Setup(r => r.UpdateAsync(It.IsAny<CleaningEvent>())).Throws<Exception>();

        var actionResult = await _controller.Update(updateRequest);

        _repository.Verify(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()), Times.Once);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<CleaningEvent>()), Times.Once);

        Assert.That(actionResult.Result, Is.TypeOf<StatusCodeResult>());
        var result = actionResult.Result as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task Update_RepositoryGetsAndUpdatesOnce_OkActionResultReturned()
    {
        var updateRequest = new CleaningEventUpdateRequest
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            Notes = "Updated",

        };
        var cleaningEvent = CleaningEvents.First();

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>())).ReturnsAsync(cleaningEvent);

        var actionResult = await _controller.Update(updateRequest);

        _repository.Verify(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()), Times.Once);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<CleaningEvent>()), Times.Once);

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        Assert.That(result.Value, Is.Not.Null.And.TypeOf<CleaningEventResponse>());
        var resultEvent = result.Value as CleaningEventResponse;
        Assert.That(resultEvent, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultEvent.Id, Is.EqualTo(cleaningEvent.Id));
            Assert.That(resultEvent.StartTime, Is.EqualTo(cleaningEvent.StartTime));
            Assert.That(resultEvent.Notes, Is.EqualTo(cleaningEvent.Notes));
        });
    }
    #endregion

    #region Delete tests
    [Test]
    public async Task Delete_RepositoryGetsAndDeletesOnce_NoContentResultReturned()
    {
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = Guid.NewGuid()
        };

        _repository.Setup(r => r.ExistsByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>())).ReturnsAsync(true);

        var actionResult = await _controller.Delete(identifyRequest);

        _repository.Verify(r => r.DeleteAsync(It.IsAny<CleaningEvent>()), Times.Once);
        _repository.Verify(r => r.ExistsByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()), Times.Once);

        Assert.That(actionResult, Is.TypeOf<NoContentResult>());
        var result = actionResult as NoContentResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
    }

    [Test]
    public async Task Delete_RepositoryReturnsNull_NotFoundActionResultReturned()
    {
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = Guid.NewGuid()
        };

        _repository.Setup(r => r.ExistsByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()))
            .ReturnsAsync(false);

        var actionResult = await _controller.Delete(identifyRequest);

        _repository.Verify(r => r.DeleteAsync(It.IsAny<CleaningEvent>()), Times.Never);
        _repository.Verify(r => r.ExistsByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()), Times.Once);

        Assert.That(actionResult, Is.TypeOf<NotFoundObjectResult>());
        var result = actionResult as NotFoundObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Delete_NullIdEntered_ValidationResultsInBadRequestResponseReturned()
    {
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = null
        };

        var actionResult = await _controller.Delete(identifyRequest);

        _repository.Verify(r => r.DeleteAsync(It.IsAny<CleaningEvent>()), Times.Never);

        Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
        var result = actionResult as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Delete_RepositoryThrowsAcquiringCleaningEvent_Status500InternalServerErrorReturned()
    {
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = Guid.NewGuid()
        };

        _repository.Setup(r => r.ExistsByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>())).Throws<Exception>();

        var actionResult = await _controller.Delete(identifyRequest);

        Assert.That(actionResult, Is.TypeOf<StatusCodeResult>());
        var result = actionResult as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));

        _repository.Verify(r => r.DeleteAsync(It.IsAny<CleaningEvent>()), Times.Never);
    }

    [Test]
    public async Task Delete_RepositoryThrowsDeletingCleaningEvent_Status500InternalServerErrorReturned()
    {
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = Guid.NewGuid()
        };

        _repository.Setup(r => r.ExistsByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>())).ReturnsAsync(true);
        _repository.Setup(r => r.DeleteAsync(It.IsAny<CleaningEvent>())).Throws<Exception>();

        var actionResult = await _controller.Delete(identifyRequest);

        Assert.That(actionResult, Is.TypeOf<StatusCodeResult>());
        var result = actionResult as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }
    #endregion
}