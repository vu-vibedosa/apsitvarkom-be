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
    private Mock<IPollutedLocationRepository> _repository;

    private readonly IEnumerable<PollutedLocation> PollutedLocations = new List<PollutedLocation>
    {
        new()
        {
            Id = Guid.NewGuid(),
            Location =
            {
                Coordinates =
                {
                  Longitude = 54,
                  Latitude = 23
                },
            },
            Radius = 15,
            Severity = PollutedLocation.SeverityLevel.Moderate,
            Spotted = DateTime.Parse("2022-09-14T17:35:23Z"),
            Progress = 42,
            Notes = "Lorem ipsum"
        },
        new()
        {
            Id = Guid.NewGuid(),
            Location =
            {
                Coordinates =
                {
                    Latitude = 111.11111,
                    Longitude = 11.11111
                },
            },
            Radius = 11,
            Severity = PollutedLocation.SeverityLevel.Low,
            Spotted = DateTime.Parse("2023-11-23T21:12:14Z"),
            Progress = 11,
            Notes = "11111"
        }
    };

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PollutedLocationProfile>();
        });
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();

        _repository = new Mock<IPollutedLocationRepository>();
        _controller = new PollutedLocationController(_repository.Object, _mapper, new CoordinatesGetRequestValidator());
    }

    #region Constructor tests
    [Test]
    public void Constructor_HappyPath_IsSuccess() => Assert.That(new PollutedLocationController(_repository.Object, _mapper, new CoordinatesGetRequestValidator()), Is.Not.Null);
    #endregion

    #region GetAll tests
    [Test]
    public async Task GetAll_RepositoryReturnsPollutedLocations_OKActionResultReturned()
    {
        _repository.Setup(self => self.GetAllAsync())
            .ReturnsAsync(PollutedLocations);

        var actionResult = await _controller.GetAll();

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(_mapper.Map<IEnumerable<PollutedLocation>>(result.Value), Is.EqualTo(PollutedLocations));
    }
    #endregion

    #region GetAllOrderedInRelationTo tests
    [Test]
    public async Task GetAllOrderedInRelationTo_RepositoryReturnsOrderedPollutedLocations_OKActionResultReturned()
    {
        var coordinatesGetRequest = new CoordinatesGetRequest
        {
            Latitude = 12.3456,
            Longitude = -65.4321
        };

        _repository.Setup(self => self.GetAllAsync(It.Is<Coordinates>(
                x => Math.Abs((double)(x.Latitude - coordinatesGetRequest.Latitude)) < 0.0001 &&
                     Math.Abs((double)(x.Longitude - coordinatesGetRequest.Longitude)) < 0.0001
            )))
            .ReturnsAsync(PollutedLocations);

        var actionResult = await _controller.GetAll(coordinatesGetRequest);

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(_mapper.Map<IEnumerable<PollutedLocation>>(result.Value), Is.EqualTo(PollutedLocations));
    }

    [Test]
    public async Task GetAllOrderedInRelationTo_ValidationFails_BadRequestActionResultReturned()
    {
        var coordinatesGetRequest = new CoordinatesGetRequest
        {
            Latitude = -91,
            Longitude = 181
        };

        _repository.Setup(self => self.GetAllAsync(It.IsAny<Coordinates>()))
            .ReturnsAsync(PollutedLocations);

        var actionResult = await _controller.GetAll(coordinatesGetRequest);

        Assert.That(actionResult.Result, Is.TypeOf<BadRequestObjectResult>());
        var result = actionResult.Result as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(result.Value, Is.TypeOf<List<string>>());

        var errorList = result.Value as List<string>;
        Assert.That(errorList, Is.Not.Null);
        Assert.That(errorList.Count, Is.EqualTo(2));
    }
    #endregion

    #region GetById tests
    [Test]
    public async Task GetById_RepositoryReturnsPollutedLocation_OKActionResultReturned()
    {
        var location = PollutedLocations.First();
        _repository.Setup(self => self.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()))
            .ReturnsAsync(_mapper.Map<PollutedLocation>(location));

        var actionResult = await _controller.GetById(location.Id.ToString());

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(_mapper.Map<PollutedLocation>(result.Value), Is.EqualTo(location));
    }

    [Test]
    public async Task GetById_RepositoryReturnsNull_NotFoundActionResultReturned()
    {
        var instanceId = Guid.NewGuid().ToString();
        _repository.Setup(self => self.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()))
            .ReturnsAsync((PollutedLocation?)null);

        var actionResult = await _controller.GetById(instanceId);

        Assert.That(actionResult.Result, Is.TypeOf<NotFoundObjectResult>());
        var result = actionResult.Result as NotFoundObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }
    #endregion
}