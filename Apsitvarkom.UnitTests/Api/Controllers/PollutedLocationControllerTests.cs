using Apsitvarkom.Api.Controllers;
using Apsitvarkom.DataAccess;
using Apsitvarkom.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Apsitvarkom.UnitTests.Api.Controllers;

public class PollutedLocationControllerTests
{
    private PollutedLocationController m_controller;
    private Mock<IPollutedLocationDTORepository> m_repository;

    private readonly IEnumerable<PollutedLocationDTO> PollutedLocationDTOs = new List<PollutedLocationDTO>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            Coordinates = new CoordinatesDTO
            {
              Longitude = 54,
              Latitude = 23
            },
            Radius = 15,
            Severity = "Moderate",
            Spotted = "2022-09-14T17:35:23Z",
            Progress = 42,
            Notes = "Lorem ipsum"
        },
        new()
        {
            Id = Guid.NewGuid().ToString(),
            Coordinates = new CoordinatesDTO
            {
                Latitude = 111.11111,
                Longitude = 11.11111
            },
            Radius = 11,
            Severity = "Low",
            Spotted = "2023-11-23T21:12:14Z",
            Progress = 11,
            Notes = "11111"
        }
    };

    [SetUp]
    public void SetUp()
    {
        m_repository = new Mock<IPollutedLocationDTORepository>();
        m_controller = new PollutedLocationController(m_repository.Object);
    }

    [Test]
    public void Constructor_HappyPath_IsSuccess() => Assert.That(new PollutedLocationController(m_repository.Object), Is.Not.Null);

    [Test]
    public async Task GetAll_RepositoryReturnsPollutedLocationDTOs_OKActionResultReturned()
    {
        m_repository.Setup(self => self.GetAllAsync()).ReturnsAsync(PollutedLocationDTOs);

        var actionResult = await m_controller.GetAll();

        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(result.Value, Is.EqualTo(PollutedLocationDTOs));
    }

    [Test]
    public async Task GetById_RepositoryReturnsPollutedLocationDTO_OKActionResultReturned()
    {
        var instance = PollutedLocationDTOs.First();
        m_repository.Setup(self => self.GetByIdAsync(instance.Id!)).ReturnsAsync(instance);

        var actionResult = await m_controller.GetById(instance.Id!);

        var result = actionResult.Result as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(result.Value, Is.EqualTo(instance));
    }

    [Test]
    public async Task GetById_RepositoryReturnsNull_NotFoundActionResultReturned()
    {
        var instanceId = Guid.NewGuid().ToString();
        m_repository.Setup(self => self.GetByIdAsync(instanceId)).ReturnsAsync((PollutedLocationDTO?)null);

        var actionResult = await m_controller.GetById(instanceId);

        var result = actionResult.Result as NotFoundObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
    }
}