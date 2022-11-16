using System.Linq.Expressions;
using Apsitvarkom.Api.Controllers;
using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Mapping;
using Apsitvarkom.Models.Public;
using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Apsitvarkom.UnitTests.Api.Controllers;

public class PollutedLocationControllerTests
{
    private PollutedLocationController m_controller;
    private IMapper m_mapper;
    private Mock<IPollutedLocationRepository> m_repository;

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
        m_mapper = config.CreateMapper();

        m_repository = new Mock<IPollutedLocationRepository>();
        m_controller = new PollutedLocationController(m_repository.Object, m_mapper, new CoordinatesGetRequestValidator());
    }

    [Test]
    public void Constructor_HappyPath_IsSuccess() => Assert.That(new PollutedLocationController(m_repository.Object, m_mapper, new CoordinatesGetRequestValidator()), Is.Not.Null);

    [Test]
    public async Task GetAll_RepositoryReturnsPollutedLocationDTOs_OKActionResultReturned()
    {
        m_repository.Setup(self => self.GetAllAsync())
            .ReturnsAsync(PollutedLocations);

        var actionResult = await m_controller.GetAll();

        Assert.That(actionResult.Result, Is.TypeOf <OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(m_mapper.Map<IEnumerable<PollutedLocation>>(result.Value), Is.EqualTo(PollutedLocations));
    }

    [Test]
    public async Task GetAllOrderedInRelationTo_RepositoryReturnsOrderedPollutedLocationDTOs_OKActionResultReturned()
    {
        var coordinatesGetRequest = new CoordinatesGetRequest
        {
            Latitude = 12.3456,
            Longitude = -65.4321
        };

        m_repository.Setup(self => self.GetAllAsync(It.Is<Coordinates>(
                x => Math.Abs(x.Latitude - coordinatesGetRequest.Latitude) < 0.0001 && Math.Abs(x.Longitude - coordinatesGetRequest.Longitude) < 0.0001)
            ))
            .ReturnsAsync(PollutedLocations);

        var actionResult = await m_controller.GetAll(coordinatesGetRequest);

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(m_mapper.Map<IEnumerable<PollutedLocation>>(result.Value), Is.EqualTo(PollutedLocations));
    }

    [Test]
    public async Task GetAllOrderedInRelationTo_ValidationFails_BadRequestActionResultReturned()
    {
        var coordinatesGetRequest = new CoordinatesGetRequest
        {
            Latitude = -91,
            Longitude = 181
        };

        m_repository.Setup(self => self.GetAllAsync(It.IsAny<Coordinates>()))
            .ReturnsAsync(PollutedLocations);

        var actionResult = await m_controller.GetAll(coordinatesGetRequest);

        Assert.That(actionResult.Result, Is.TypeOf<BadRequestObjectResult>());
        var result = actionResult.Result as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(result.Value, Is.TypeOf<List<ValidationFailure>>());

        var errorList = result.Value as List<ValidationFailure>;
        Assert.That(errorList, Is.Not.Null);
        Assert.That(errorList.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetById_RepositoryReturnsPollutedLocationDTO_OKActionResultReturned()
    {
        var location = PollutedLocations.First();
        m_repository.Setup(self => self.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()))
            .ReturnsAsync(m_mapper.Map<PollutedLocation>(location));

        var actionResult = await m_controller.GetById(location.Id.ToString());

        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(m_mapper.Map<PollutedLocation>(result.Value), Is.EqualTo(location));
    }

    [Test]
    public async Task GetById_RepositoryReturnsNull_NotFoundActionResultReturned()
    {
        var instanceId = Guid.NewGuid().ToString();
        m_repository.Setup(self => self.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()))
            .ReturnsAsync((PollutedLocation?)null);

        var actionResult = await m_controller.GetById(instanceId);

        Assert.That(actionResult.Result, Is.TypeOf<NotFoundObjectResult>());
        var result = actionResult.Result as NotFoundObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
    }
}