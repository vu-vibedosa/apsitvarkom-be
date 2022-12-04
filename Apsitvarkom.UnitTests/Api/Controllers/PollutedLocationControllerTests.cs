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

public class PollutedLocationControllerTests
{
    private PollutedLocationController _controller;
    private IMapper _mapper;
    private Mock<IGeocoder> _geocoder;
    private Mock<IPollutedLocationRepository> _repository;

    private readonly IEnumerable<PollutedLocation> PollutedLocations = new List<PollutedLocation>
    {
        new()
        {
            Id = Guid.Parse("7df570d5-efbb-4bf5-a21c-b9d33dafca36"),
            Location =
            {
                Title = new("title1", "pavadinimas1"),
                Coordinates =
                {
                    Longitude = 54,
                    Latitude = 23
                }
            },
            Radius = 15,
            Severity = PollutedLocation.SeverityLevel.Moderate,
            Spotted = DateTime.Parse("2022-09-14T17:35:23Z"),
            Progress = 42,
            Notes = "Lorem ipsum",
            Events = new List<CleaningEvent>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    PollutedLocationId = Guid.Parse("7df570d5-efbb-4bf5-a21c-b9d33dafca36"),
                    StartTime = DateTime.Parse("2023-01-01T00:11:22Z"),
                    Notes = "So many fireworks leftovers..."
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    PollutedLocationId = Guid.Parse("7df570d5-efbb-4bf5-a21c-b9d33dafca36"),
                    StartTime = DateTime.Parse("2022-12-23T10:11:12Z"),
                },
            }
        },
        new()
        {
            Id = Guid.NewGuid(),
            Location =
            {
                Title = new("title2", "pavadinimas2"),
                Coordinates =
                {
                    Latitude = 11.11111,
                    Longitude = 111.11111
                }
            },
            Radius = 11,
            Severity = PollutedLocation.SeverityLevel.Low,
            Spotted = DateTime.Parse("2023-11-23T21:12:14Z"),
            Progress = 11,
            Notes = "11111",
            Events = new List<CleaningEvent>()
        }
    };

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PollutedLocationProfile>();
            cfg.AddProfile<CleaningEventProfile>();
        });
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();

        _geocoder = new Mock<IGeocoder>();
        _repository = new Mock<IPollutedLocationRepository>();
        _controller = new PollutedLocationController(
            _repository.Object,
            _mapper,
            _geocoder.Object,
            new CoordinatesCreateRequestValidator(),
            new PollutedLocationCreateRequestValidator(new LocationCreateRequestValidator(new CoordinatesCreateRequestValidator())),
            new ObjectIdentifyRequestValidator(),
            new PollutedLocationUpdateRequestValidator()
        );
    }

    #region Constructor tests
    [Test]
    public void Constructor_HappyPath_IsSuccess() => Assert.That(new PollutedLocationController(
            _repository.Object,
            _mapper,
            _geocoder.Object,
            new CoordinatesCreateRequestValidator(),
            new PollutedLocationCreateRequestValidator(new LocationCreateRequestValidator(new CoordinatesCreateRequestValidator())),
            new ObjectIdentifyRequestValidator(),
            new PollutedLocationUpdateRequestValidator()
        ), Is.Not.Null);
    #endregion

    #region GetAll tests
    [Test]
    public async Task GetAll_RepositoryReturnsPollutedLocations_OKActionResultReturned()
    {
        _repository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(PollutedLocations);

        var actionResult = await _controller.GetAll();

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        Assert.That(result.Value, Is.Not.Null.And.InstanceOf<IEnumerable<PollutedLocationResponse>>());
        var resultLocations = result.Value as IEnumerable<PollutedLocationResponse>;
        Assert.That(resultLocations, Is.Not.Null.And.Count.EqualTo(PollutedLocations.Count()));

        var sortedRepositoryLocations = PollutedLocations.OrderBy(o => o.Spotted);

        for (var i = 0; i < PollutedLocations.Count(); i++)
        {
            var location = sortedRepositoryLocations.ElementAt(i);
            var resultLocation = resultLocations.ElementAt(i);
            Assert.Multiple(() =>
            {
                Assert.That(resultLocation.Id, Is.EqualTo(location.Id));
                Assert.That(resultLocation.Spotted, Is.EqualTo(location.Spotted));
                Assert.That(resultLocation.Radius, Is.EqualTo(location.Radius));
                Assert.That(resultLocation.Severity, Is.EqualTo(location.Severity));
                Assert.That(resultLocation.Progress, Is.EqualTo(location.Progress));
                Assert.That(resultLocation.Location.Title.En, Is.EqualTo(location.Location.Title.English));
                Assert.That(resultLocation.Location.Title.Lt, Is.EqualTo(location.Location.Title.Lithuanian));
                Assert.That(resultLocation.Location.Coordinates.Latitude, Is.EqualTo(location.Location.Coordinates.Latitude));
                Assert.That(resultLocation.Location.Coordinates.Longitude, Is.EqualTo(location.Location.Coordinates.Longitude));
                Assert.That(resultLocation.Notes, Is.EqualTo(location.Notes));
                Assert.That(resultLocation.Events.Count, Is.EqualTo(location.Events.Count));
                for (var j = 0; j < resultLocation.Events.Count; ++j)
                {
                    Assert.That(resultLocation.Events[j].PollutedLocationId, Is.EqualTo(location.Events[j].PollutedLocationId));
                    Assert.That(resultLocation.Events[j].Id, Is.EqualTo(location.Events[j].Id));
                    Assert.That(resultLocation.Events[j].Notes, Is.EqualTo(location.Events[j].Notes));
                    Assert.That(resultLocation.Events[j].StartTime, Is.EqualTo(location.Events[j].StartTime));
                }
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

    #region GetAllOrderedInRelationTo tests
    [Test]
    public async Task GetAllOrderedInRelationTo_RepositoryReturnsOrderedPollutedLocations_OKActionResultReturned()
    {
        var coordinatesCreateRequest = new CoordinatesCreateRequest
        {
            Latitude = 12.3456,
            Longitude = -65.4321
        };

        _repository.Setup(r => r.GetAllAsync(It.Is<Coordinates>(x =>
                Math.Abs((double)(x.Latitude - coordinatesCreateRequest.Latitude)) < 0.0001 &&
                Math.Abs((double)(x.Longitude - coordinatesCreateRequest.Longitude)) < 0.0001
            )))
            .ReturnsAsync(PollutedLocations);

        var actionResult = await _controller.GetAll(coordinatesCreateRequest);

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        Assert.That(result.Value, Is.Not.Null.And.InstanceOf<IEnumerable<PollutedLocationResponse>>());
        var resultLocations = result.Value as IEnumerable<PollutedLocationResponse>;
        Assert.That(resultLocations, Is.Not.Null.And.Count.EqualTo(PollutedLocations.Count()));
        for (var i = 0; i < PollutedLocations.Count(); i++)
        {
            var location = PollutedLocations.ElementAt(i);
            var resultLocation = resultLocations.ElementAt(i);
            Assert.Multiple(() =>
            {
                Assert.That(resultLocation.Id, Is.EqualTo(location.Id));
                Assert.That(resultLocation.Spotted, Is.EqualTo(location.Spotted));
                Assert.That(resultLocation.Radius, Is.EqualTo(location.Radius));
                Assert.That(resultLocation.Severity, Is.EqualTo(location.Severity));
                Assert.That(resultLocation.Progress, Is.EqualTo(location.Progress));
                Assert.That(resultLocation.Location.Title.En, Is.EqualTo(location.Location.Title.English));
                Assert.That(resultLocation.Location.Title.Lt, Is.EqualTo(location.Location.Title.Lithuanian));
                Assert.That(resultLocation.Location.Coordinates.Latitude, Is.EqualTo(location.Location.Coordinates.Latitude));
                Assert.That(resultLocation.Location.Coordinates.Longitude, Is.EqualTo(location.Location.Coordinates.Longitude));
                Assert.That(resultLocation.Events.Count, Is.EqualTo(location.Events.Count));
                for (var j = 0; j < resultLocation.Events.Count; ++j)
                {
                    Assert.That(resultLocation.Events[j].PollutedLocationId, Is.EqualTo(location.Events[j].PollutedLocationId));
                    Assert.That(resultLocation.Events[j].Id, Is.EqualTo(location.Events[j].Id));
                    Assert.That(resultLocation.Events[j].Notes, Is.EqualTo(location.Events[j].Notes));
                    Assert.That(resultLocation.Events[j].StartTime, Is.EqualTo(location.Events[j].StartTime));
                }
            });
        }
    }

    [Test]
    public async Task GetAllOrderedInRelationTo_ValidationFails_BadRequestActionResultReturned()
    {
        var coordinatesCreateRequest = new CoordinatesCreateRequest
        {
            Latitude = -91,
            Longitude = 181
        };

        _repository.Setup(r => r.GetAllAsync(It.IsAny<Coordinates>()))
            .ReturnsAsync(PollutedLocations);

        var actionResult = await _controller.GetAll(coordinatesCreateRequest);

        Assert.That(actionResult.Result, Is.TypeOf<BadRequestObjectResult>());
        var result = actionResult.Result as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(result.Value, Is.TypeOf<List<string>>());

        var errorList = result.Value as List<string>;
        Assert.That(errorList, Is.Not.Null);
        Assert.That(errorList.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetAllOrderedInRelationTo_RepositoryThrows_Status500InternalServerErrorReturned()
    {
        var coordinatesRequest = new CoordinatesCreateRequest
        {
            Latitude = 64.12312,
            Longitude = -12.4123
        };
        _repository.Setup(r => r.GetAllAsync(It.IsAny<Coordinates>()))
            .Throws<Exception>();

        var actionResult = await _controller.GetAll(coordinatesRequest);

        Assert.That(actionResult.Result, Is.TypeOf<StatusCodeResult>());
        var result = actionResult.Result as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }
    #endregion

    #region GetById tests
    [Test]
    public async Task GetById_RepositoryReturnsPollutedLocation_OKActionResultReturned()
    {
        var location = PollutedLocations.First();
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = location.Id
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()))
            .ReturnsAsync(location);

        var actionResult = await _controller.GetById(identifyRequest);

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        Assert.That(result.Value, Is.Not.Null.And.TypeOf<PollutedLocationResponse>());
        var resultLocation = result.Value as PollutedLocationResponse;
        Assert.That(resultLocation, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultLocation.Id, Is.EqualTo(location.Id));
            Assert.That(resultLocation.Spotted, Is.EqualTo(location.Spotted));
            Assert.That(resultLocation.Radius, Is.EqualTo(location.Radius));
            Assert.That(resultLocation.Severity, Is.EqualTo(location.Severity));
            Assert.That(resultLocation.Progress, Is.EqualTo(location.Progress));
            Assert.That(resultLocation.Location.Title.En, Is.EqualTo(location.Location.Title.English));
            Assert.That(resultLocation.Location.Title.Lt, Is.EqualTo(location.Location.Title.Lithuanian));
            Assert.That(resultLocation.Location.Coordinates.Latitude, Is.EqualTo(location.Location.Coordinates.Latitude));
            Assert.That(resultLocation.Location.Coordinates.Longitude, Is.EqualTo(location.Location.Coordinates.Longitude));
            Assert.That(resultLocation.Events.Count, Is.EqualTo(location.Events.Count));
            for (var j = 0; j < resultLocation.Events.Count; ++j)
            {
                Assert.That(resultLocation.Events[j].PollutedLocationId, Is.EqualTo(location.Events[j].PollutedLocationId));
                Assert.That(resultLocation.Events[j].Id, Is.EqualTo(location.Events[j].Id));
                Assert.That(resultLocation.Events[j].Notes, Is.EqualTo(location.Events[j].Notes));
                Assert.That(resultLocation.Events[j].StartTime, Is.EqualTo(location.Events[j].StartTime));
            }
        });
    }

    [Test]
    public async Task GetById_RepositoryReturnsNull_NotFoundActionResultReturned()
    {
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = Guid.NewGuid()
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()))
            .ReturnsAsync((PollutedLocation?)null);

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

        _repository.Verify(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Never);

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

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()))
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
        var location = PollutedLocations.First();
        var createRequest = new PollutedLocationCreateRequest
        {
            Location = new LocationCreateRequest
            {
                Coordinates = new CoordinatesCreateRequest
                {
                    Latitude = location.Location.Coordinates.Latitude,
                    Longitude = location.Location.Coordinates.Longitude
                }
            },
            Radius = location.Radius,
            Severity = location.Severity
        };

        var titleResult = new Translated<string>("englishman in new york", "lietuvis kaune");
        _geocoder.Setup(g => g.ReverseGeocodeAsync(It.Is<Coordinates>(x =>
            Math.Abs(x.Latitude - location.Location.Coordinates.Latitude) < 0.0001 &&
            Math.Abs(x.Longitude - location.Location.Coordinates.Longitude) < 0.0001
        ))).ReturnsAsync(titleResult);

        var actionResult = await _controller.Create(createRequest);

        _repository.Verify(r => r.InsertAsync(It.IsAny<PollutedLocation>()), Times.Once);
        _geocoder.Verify(g => g.ReverseGeocodeAsync(It.IsAny<Coordinates>()), Times.Once);

        Assert.That(actionResult.Result, Is.TypeOf<CreatedAtActionResult>());
        var result = actionResult.Result as CreatedAtActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));

        Assert.That(result.Value, Is.Not.Null.And.TypeOf<PollutedLocationResponse>());
        var resultLocation = result.Value as PollutedLocationResponse;
        Assert.That(resultLocation, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultLocation.Radius, Is.EqualTo(createRequest.Radius));
            Assert.That(resultLocation.Severity, Is.EqualTo(createRequest.Severity));
            Assert.That(resultLocation.Location.Coordinates.Latitude, Is.EqualTo(createRequest.Location.Coordinates.Latitude));
            Assert.That(resultLocation.Location.Coordinates.Longitude, Is.EqualTo(createRequest.Location.Coordinates.Longitude));
        });
    }

    [Test]
    public async Task Create_InvalidDataEntered_ValidationResultsInBadRequestResponseReturned()
    {
        var createRequest = new PollutedLocationCreateRequest
        {
            Severity = null
        };

        var actionResult = await _controller.Create(createRequest);

        _repository.Verify(r => r.InsertAsync(It.IsAny<PollutedLocation>()), Times.Never);

        Assert.That(actionResult.Result, Is.TypeOf<BadRequestObjectResult>());
        var result = actionResult.Result as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Create_RepositoryThrows_Status500InternalServerErrorReturned()
    {
        var location = PollutedLocations.First();
        var createRequest = new PollutedLocationCreateRequest
        {
            Location = new LocationCreateRequest
            {
                Coordinates = new CoordinatesCreateRequest
                {
                    Latitude = location.Location.Coordinates.Latitude,
                    Longitude = location.Location.Coordinates.Longitude
                }
            },
            Radius = location.Radius,
            Severity = location.Severity
        };

        var titleResult = new Translated<string>("englishman in new york", "lietuvis kaune");
        _geocoder.Setup(g => g.ReverseGeocodeAsync(It.Is<Coordinates>(x =>
            Math.Abs(x.Latitude - location.Location.Coordinates.Latitude) < 0.0001 &&
            Math.Abs(x.Longitude - location.Location.Coordinates.Longitude) < 0.0001
        ))).ReturnsAsync(titleResult);

        _repository.Setup(r => r.InsertAsync(It.IsAny<PollutedLocation>())).Throws<Exception>();

        var actionResult = await _controller.Create(createRequest);

        Assert.That(actionResult.Result, Is.TypeOf<StatusCodeResult>());
        var result = actionResult.Result as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));

        _geocoder.Verify(g => g.ReverseGeocodeAsync(It.IsAny<Coordinates>()), Times.Once);
    }
    #endregion

    #region Update tests
    [Test]
    public async Task Update_NullIdEntered_ValidationResultsInBadRequestResponseReturned()
    {
        var updateRequest = new PollutedLocationUpdateRequest
        {
            Id = null
        };

        var actionResult = await _controller.Update(updateRequest);

        _repository.Verify(r => r.UpdateAsync(It.IsAny<PollutedLocation>()), Times.Never);

        Assert.That(actionResult.Result, Is.TypeOf<BadRequestObjectResult>());
        var result = actionResult.Result as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Update_RepositoryThrowsAcquiringPollutedLocation_Status500InternalServerErrorReturned()
    {
        var updateRequest = new PollutedLocationUpdateRequest
        {
            Id = Guid.NewGuid(),
            Progress = 12,
            Radius = 3,
            Severity = PollutedLocation.SeverityLevel.High
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).Throws<Exception>();

        var actionResult = await _controller.Update(updateRequest);

        _repository.Verify(r => r.UpdateAsync(It.IsAny<PollutedLocation>()), Times.Never);

        Assert.That(actionResult.Result, Is.TypeOf<StatusCodeResult>());
        var result = actionResult.Result as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task Update_RepositoryReturnsNullAcquiringInstance_NotFoundActionResultReturned()
    {
        var updateRequest = new PollutedLocationUpdateRequest
        {
            Id = Guid.NewGuid(),
            Progress = 12,
            Radius = 3,
            Severity = PollutedLocation.SeverityLevel.High
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).ReturnsAsync((PollutedLocation?)null);

        var actionResult = await _controller.Update(updateRequest);

        _repository.Verify(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Once);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<PollutedLocation>()), Times.Never);

        Assert.That(actionResult.Result, Is.TypeOf<NotFoundObjectResult>());
        var result = actionResult.Result as NotFoundObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Update_RepositoryThrowsUpdatingPollutedLocation_Status500InternalServerErrorReturned()
    {
        var updateRequest = new PollutedLocationUpdateRequest
        {
            Id = Guid.NewGuid(),
            Progress = 12,
            Radius = 3,
            Severity = PollutedLocation.SeverityLevel.High
        };

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).ReturnsAsync(PollutedLocations.First());
        _repository.Setup(r => r.UpdateAsync(It.IsAny<PollutedLocation>())).Throws<Exception>();

        var actionResult = await _controller.Update(updateRequest);

        _repository.Verify(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Once);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<PollutedLocation>()), Times.Once);

        Assert.That(actionResult.Result, Is.TypeOf<StatusCodeResult>());
        var result = actionResult.Result as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task Update_RepositoryGetsAndUpdatesOnce_OkActionResultReturned()
    {
        var updateRequest = new PollutedLocationUpdateRequest
        {
            Id = Guid.NewGuid(),
            Radius = 3,
            Severity = PollutedLocation.SeverityLevel.High
        };
        var location = PollutedLocations.First();

        _repository.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).ReturnsAsync(location);

        var actionResult = await _controller.Update(updateRequest);

        _repository.Verify(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Once);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<PollutedLocation>()), Times.Once);

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        Assert.That(result.Value, Is.Not.Null.And.TypeOf<PollutedLocationResponse>());
        var resultLocation = result.Value as PollutedLocationResponse;
        Assert.That(resultLocation, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultLocation.Id, Is.EqualTo(location.Id));
            Assert.That(resultLocation.Spotted, Is.EqualTo(location.Spotted));
            Assert.That(resultLocation.Radius, Is.EqualTo(updateRequest.Radius));
            Assert.That(resultLocation.Severity, Is.EqualTo(updateRequest.Severity));
            Assert.That(resultLocation.Progress, Is.EqualTo(location.Progress));
            Assert.That(resultLocation.Location.Title.En, Is.EqualTo(location.Location.Title.English));
            Assert.That(resultLocation.Location.Title.Lt, Is.EqualTo(location.Location.Title.Lithuanian));
            Assert.That(resultLocation.Location.Coordinates.Latitude, Is.EqualTo(location.Location.Coordinates.Latitude));
            Assert.That(resultLocation.Location.Coordinates.Longitude, Is.EqualTo(location.Location.Coordinates.Longitude));
            Assert.That(resultLocation.Events.Count, Is.EqualTo(location.Events.Count));
            for (var j = 0; j < resultLocation.Events.Count; ++j)
            {
                Assert.That(resultLocation.Events[j].PollutedLocationId, Is.EqualTo(location.Events[j].PollutedLocationId));
                Assert.That(resultLocation.Events[j].Id, Is.EqualTo(location.Events[j].Id));
                Assert.That(resultLocation.Events[j].Notes, Is.EqualTo(location.Events[j].Notes));
                Assert.That(resultLocation.Events[j].StartTime, Is.EqualTo(location.Events[j].StartTime));
            }
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

        _repository.Setup(r => r.ExistsByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).ReturnsAsync(true);

        var actionResult = await _controller.Delete(identifyRequest);

        _repository.Verify(r => r.DeleteAsync(It.IsAny<PollutedLocation>()), Times.Once);
        _repository.Verify(r => r.ExistsByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Once);

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

        _repository.Setup(r => r.ExistsByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()))
            .ReturnsAsync(false);

        var actionResult = await _controller.Delete(identifyRequest);

        _repository.Verify(r => r.DeleteAsync(It.IsAny<PollutedLocation>()), Times.Never);
        _repository.Verify(r => r.ExistsByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Once);

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

        _repository.Verify(r => r.DeleteAsync(It.IsAny<PollutedLocation>()), Times.Never);

        Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
        var result = actionResult as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Delete_RepositoryThrowsAcquiringPollutedLocation_Status500InternalServerErrorReturned()
    {
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = Guid.NewGuid()
        };

        _repository.Setup(r => r.ExistsByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).Throws<Exception>();

        var actionResult = await _controller.Delete(identifyRequest);

        Assert.That(actionResult, Is.TypeOf<StatusCodeResult>());
        var result = actionResult as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));

        _repository.Verify(r => r.DeleteAsync(It.IsAny<PollutedLocation>()), Times.Never);
    }

    [Test]
    public async Task Delete_RepositoryThrowsDeletingPollutedLocation_Status500InternalServerErrorReturned()
    {
        var identifyRequest = new ObjectIdentifyRequest
        {
            Id = Guid.NewGuid()
        };

        _repository.Setup(r => r.ExistsByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).ReturnsAsync(true);
        _repository.Setup(r => r.DeleteAsync(It.IsAny<PollutedLocation>())).Throws<Exception>();

        var actionResult = await _controller.Delete(identifyRequest);

        Assert.That(actionResult, Is.TypeOf<StatusCodeResult>());
        var result = actionResult as StatusCodeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }
    #endregion
}